using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly StreamerManager _streamerManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;

        public SubscriptionsManager(
            StreamerManager streamerManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamerManager = streamerManager;
            _subscriptionsPersistence = subscriptionsPersistence;
            _subscriptions = new ConcurrentDictionary<Subscription, IDisposable>();
        }

        public IDictionary<Subscription, IDisposable> Get()
        {
            return _subscriptions;
        }

        public void AddOrUpdate(Subscription subscription)
        {
            _streamerManager.AddOrUpdate(subscription);

            SubscriptionEntity entity = _subscriptionsPersistence.Get(subscription.Id, subscription.Platform);
            _subscriptionsPersistence.AddOrUpdate(entity);
        }

        public void Remove(Subscription subscription)
        {
            _streamerManager.Remove(subscription);
            
            SubscriptionEntity entity = _subscriptionsPersistence.Get(subscription.Id, subscription.Platform);
            _subscriptionsPersistence.Remove(entity);
        }
    }
}