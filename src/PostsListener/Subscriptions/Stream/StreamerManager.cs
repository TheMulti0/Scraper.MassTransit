using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class StreamerManager
    {
        private readonly PostsStreamer _streamer;
        private readonly IBus _bus;
        private readonly Dictionary<string, double> _intervalMultipliers;
        private readonly ConcurrentDictionary<Subscription, PostSubscription> _subscriptions;
        private readonly ILogger<StreamerManager> _logger;

        public StreamerManager(
            StreamerManagerConfig config,
            PostsStreamer streamer,
            IBus bus,
            ILogger<StreamerManager> logger)
        {
            _streamer = streamer;
            _bus = bus;
            _intervalMultipliers = config.PlatformMultipliers;
            _subscriptions = new ConcurrentDictionary<Subscription, PostSubscription>();
            _logger = logger;
        }

        public IDictionary<Subscription, PostSubscription> Get()
        {
            return _subscriptions;
        }

        public void AddOrUpdate(Subscription subscription, DateTime earliestPostDate)
        {
            if (_subscriptions.ContainsKey(subscription))
            {
                if (_subscriptions.FirstOrDefault(pair => pair.Key == subscription)
                    .Key.PollInterval != subscription.PollInterval)
                {
                    _subscriptions.Remove(subscription, out PostSubscription s);
                    s?.Dispose();
                }
            }

            _subscriptions.GetOrAdd(
                subscription,
                s => StreamSubscription(s, earliestPostDate));    
        }

        private PostSubscription StreamSubscription(Subscription subscription, DateTime earliestPostDate)
        {
            var trigger = new Subject<Unit>();
            var disposable = Subscribe(subscription, trigger, earliestPostDate);

            return new PostSubscription(trigger, disposable);
        }

        private IDisposable Subscribe(
            Subscription subscription,
            IObservable<Unit> trigger,
            DateTime earliestPostDate)
        {
            string id = subscription.Id;
            string platform = subscription.Platform;
            double intervalMultiplier = GetPlatformIntervalMultiplier(platform);
            TimeSpan interval = subscription.PollInterval * intervalMultiplier;

            _logger.LogInformation("Streaming [{}] {} with interval of {}", platform, id, interval);

            IObservable<Post> stream = _streamer
                .Stream(id, platform, interval, trigger)
                .Where(post => post.CreationDate > earliestPostDate);

            return stream.SubscribeAsync(post => PublishPost(platform, post));
        }

        private async Task PublishPost(string platform, Post post)
        {
            _logger.LogInformation("Sending {}", post.Url);

            await _bus.Publish(
                new NewPost
                {
                    Post = post,
                    Platform = platform
                });
        }

        private double GetPlatformIntervalMultiplier(string platform)
        {
            return _intervalMultipliers.ContainsKey(platform)
                ? _intervalMultipliers[platform]
                : 1;
        }

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.ContainsKey(subscription))
            {
                throw new KeyNotFoundException();
            }

            if (!_subscriptions.TryRemove(subscription, out PostSubscription postSubscription))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }

            postSubscription?.Dispose();
        }
    }
}