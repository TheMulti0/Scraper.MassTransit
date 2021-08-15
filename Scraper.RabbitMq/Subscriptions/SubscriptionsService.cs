using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class SubscriptionsService : IHostedService
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsService(
            ISubscriptionsManager subscriptionsManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _subscriptionsManager = subscriptionsManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Subscription> subscriptions = _subscriptionsPersistence.Get();
            
            foreach (Subscription subscription in subscriptions)
            {
                _subscriptionsManager.AddOrUpdate(subscription);
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IDisposable disposable in _subscriptionsManager.Get().Values)
            {
                disposable.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}