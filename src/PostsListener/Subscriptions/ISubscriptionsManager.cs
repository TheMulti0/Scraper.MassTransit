using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public interface ISubscriptionsManager
    {
        IEnumerable<Subscription> Get();
        
        Task AddOrUpdateAsync(Subscription subscription, CancellationToken ct);

        Task RemoveAsync(Subscription subscription, CancellationToken ct);
    }
}