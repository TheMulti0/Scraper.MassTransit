using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public interface ISubscriptionsClient
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct = default);

        Task SubscribeAsync(string platform, string id, TimeSpan pollInterval, CancellationToken ct = default);

        Task UnsubscribeAsync(string platform, string id, CancellationToken ct = default);
    }
}