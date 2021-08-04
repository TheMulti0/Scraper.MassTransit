using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public SubscriptionsManager(
            PostsStreamer streamer,
            IPostsPublisher publisher)
        {
            _streamer = streamer;
            _publisher = publisher;
            _subscriptions = new ConcurrentDictionary<Subscription, IDisposable>();
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
                .Stream(subscription.Id, subscription.Platform, subscription.PollInterval);

            var consumer = new PostsConsumer(subscription, _publisher);
            
            IDisposable disposable = stream.Subscribe(consumer.OnPost);

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