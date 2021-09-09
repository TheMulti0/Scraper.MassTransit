using System.Collections.Generic;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit
{
    public interface ISubscriptionsPersistence
    {
        IEnumerable<Subscription> Get();
        
        void AddOrUpdate(Subscription subscription);

        void Remove(Subscription subscription);
    }
}