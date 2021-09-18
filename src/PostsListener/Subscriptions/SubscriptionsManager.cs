using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly StreamerManager _streamerManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsManager(
            StreamerManager streamerManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamerManager = streamerManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public IEnumerable<Subscription> Get()
        {
            return _streamerManager.Get().Keys;
        }

        public async Task AddOrUpdateAsync(
            Subscription subscription,
            DateTime? earliestPostDate = null,
            CancellationToken ct = default)
        {
            _streamerManager.AddOrUpdate(subscription, earliestPostDate ?? DateTime.MinValue);

            SubscriptionEntity entity = await ToEntity(subscription, ct);

            await _subscriptionsPersistence.AddOrUpdateAsync(entity, ct);
        }

        private async Task<SubscriptionEntity> ToEntity(Subscription subscription, CancellationToken ct)
        {
            SubscriptionEntity existing = await _subscriptionsPersistence.GetAsync(
                subscription.Id,
                subscription.Platform,
                ct);

            SubscriptionEntity entity = subscription.ToNewEntity();

            if (existing == null)
            {
                return entity;
            }
            
            return entity with { SubscriptionId = existing.SubscriptionId };
        }

        public async Task RemoveAsync(Subscription subscription, CancellationToken ct)
        {
            _streamerManager.Remove(subscription);
            
            SubscriptionEntity entity = await _subscriptionsPersistence.GetAsync(subscription.Id, subscription.Platform, ct);
            
            await _subscriptionsPersistence.RemoveAsync(entity, ct);
        }
    }
}