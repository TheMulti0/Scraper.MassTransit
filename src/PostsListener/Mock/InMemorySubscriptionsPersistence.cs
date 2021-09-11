using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class InMemorySubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly object _subscriptionsLock = new();
        private readonly List<SubscriptionEntity> _subscriptions = new();
        private readonly ILogger<InMemorySubscriptionsPersistence> _logger;

        public InMemorySubscriptionsPersistence(ILogger<InMemorySubscriptionsPersistence> logger)
        {
            _logger = logger;
        }

        public IEnumerable<SubscriptionEntity> Get()
        {
            lock (_subscriptionsLock)
            {
                return _subscriptions.ToArray();
            }
        }

        public SubscriptionEntity Get(string id, string platform)
        {
            lock (_subscriptionsLock)
            {
                return _subscriptions.Find(entity => entity.Id == id && entity.Platform == platform);
            }
        }

        public void AddOrUpdate(SubscriptionEntity subscription)
        {
            lock (_subscriptionsLock)
            {
                _subscriptions.Add(subscription);
            }
            
            _logger.LogInformation("Added subscription [{}] {}", subscription.Platform, subscription.Id);
        }

        public void Remove(SubscriptionEntity subscription)
        {
            lock (_subscriptionsLock)
            {
                if (!_subscriptions.Remove(subscription))
                {
                    throw new InvalidOperationException("Failed to remove subscription");
                }    
            }
            
            _logger.LogInformation("Removed subscription [{}] {}", subscription.Platform, subscription.Id);
        }
    }
}