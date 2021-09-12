using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class SubscriptionsLoaderService : IHostedService
    {
        private readonly StreamerManager _streamerManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;

        public SubscriptionsLoaderService(
            StreamerManager streamerManager,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamerManager = streamerManager;
            _subscriptionsPersistence = subscriptionsPersistence;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            IAsyncEnumerable<Subscription> subscriptions = _subscriptionsPersistence
                .GetAsync(ct)
                .Select(entity => entity.ToSubscription());
            
            await foreach (Subscription subscription in subscriptions.WithCancellation(ct))
            {
                _streamerManager.AddOrUpdate(subscription, DateTime.MinValue);
            }
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