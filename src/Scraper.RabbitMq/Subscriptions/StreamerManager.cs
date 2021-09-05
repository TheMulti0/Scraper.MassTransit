using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class StreamerManager : ISubscriptionsManager
    {
        private readonly PostsStreamer _streamer;
        private readonly IBus _bus;
        private readonly Dictionary<string, int> _intervalMultipliers;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;
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
            _subscriptions = new ConcurrentDictionary<Subscription, IDisposable>();
            _logger = logger;
        }

        public IDictionary<Subscription, IDisposable> Get()
        {
            return _subscriptions;
        }

        public void AddOrUpdate(Subscription subscription)
        {
            _subscriptions.AddOrUpdate(
                subscription,
                StreamSubscription,
                (s, old) =>
                {
                    old.Dispose();
                    return StreamSubscription(s);
                });
        }

        private IDisposable StreamSubscription(Subscription subscription)
        {
            string id = subscription.Id;
            string platform = subscription.Platform;
            int intervalMultiplier = GetPlatformIntervalMultiplier(platform);
            TimeSpan interval = subscription.PollInterval * intervalMultiplier;
            
            _logger.LogInformation("Streaming [{}] {} with interval of {}", platform, id, interval);

            IObservable<Post> stream = _streamer
                .Stream(id, platform, interval);

            async Task PublishPost(Post post)
            {
                _logger.LogInformation("Sending {}", post.Url);
                    
                await _bus.Publish(
                    new NewPost
                    {
                        Post = post,
                        Platform = platform
                    });
            }

            return stream.SubscribeAsync(PublishPost);
        }

        private int GetPlatformIntervalMultiplier(string platform)
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
        
            if (!_subscriptions.TryRemove(subscription, out IDisposable disposable))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            disposable?.Dispose();
        }
    }
}