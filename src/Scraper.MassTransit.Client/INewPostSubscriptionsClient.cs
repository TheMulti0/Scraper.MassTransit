using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Client
{
    public interface INewPostSubscriptionsClient
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct = default);

        Task AddOrUpdateSubscription(string id, string platform, TimeSpan pollInterval, CancellationToken ct = default);

        Task RemoveSubscription(string id, string platform, CancellationToken ct = default);
    }
}