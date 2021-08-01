using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Scraper.Net;
using Scraper.Net.Stream;

namespace Scraper.RabbitMq
{
    public class SubscriptionsService : IHostedService
    {
        private readonly ISubscriptionsManager _subscriptionsManager;

        public SubscriptionsService(ISubscriptionsManager subscriptionsManager)
        {
            _subscriptionsManager = subscriptionsManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
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