using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly PostsStreamer _streamer;
        private readonly IPostsPublisher _publisher;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;
        private readonly TaskPoolScheduler _scheduler;

        public SubscriptionsManager(
            PostsStreamer streamer,
            IPostsPublisher publisher,
            SubscriptionsManagerConfig config)
        {
            _streamer = streamer;
            _publisher = publisher;
            _subscriptions = new ConcurrentDictionary<Subscription, IDisposable>();

            _scheduler = new TaskPoolScheduler(
                new TaskFactory(
                    new LimitedConcurrencyLevelTaskScheduler(config.MaxDegreeOfParallelism)));
        }

        public IDictionary<Subscription, IDisposable> Get()
        {
            return _subscriptions;
        }

        public void Add(Subscription subscription)
        {
            if (_subscriptions.ContainsKey(subscription))
            {
                throw new InvalidOperationException("Subscription already exists");
            }
            
            IObservable<Post> stream = _streamer
                .Stream(subscription.Id, subscription.Platform, subscription.PollInterval, _scheduler);

            IDisposable disposable = stream.Subscribe(
                post => _publisher.Send(post, subscription.Platform));

            if (!_subscriptions.TryAdd(subscription, disposable))
            {
                throw new InvalidOperationException("Failed to add subscription");
            }
        }

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.TryRemove(subscription, out IDisposable disposable))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            disposable?.Dispose();
        }
    }
}