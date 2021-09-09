using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsService : IHostedService
    {
        private readonly StreamerManager _streamerManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsService(
            StreamerManager streamerManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamerManager = streamerManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Subscription> subscriptions = _subscriptionsPersistence.Get();
            
            foreach (Subscription subscription in subscriptions)
            {
                _streamerManager.AddOrUpdate(subscription);
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (IDisposable disposable in _streamerManager.Get().Values)
            {
                disposable.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}