using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.Net.Stream;

namespace Scraper.RabbitMq
{
    public class InMemorySubscriptionsManager : ISubscriptionsManager
    {
        private readonly PostsStreamer _streamer;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;

        public InMemorySubscriptionsManager(PostsStreamer streamer)
        {
            _streamer = streamer;
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

            var consumer = new PostsConsumer(subscription);
            
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