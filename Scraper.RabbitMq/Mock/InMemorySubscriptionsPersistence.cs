using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class InMemorySubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly ConcurrentBag<Subscription> _subscriptions = new();
        
        public IEnumerable<Subscription> Get() => _subscriptions;

        public void Add(Subscription subscription) => _subscriptions.Add(subscription);

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.TryTake(out subscription))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
        }
    }
}