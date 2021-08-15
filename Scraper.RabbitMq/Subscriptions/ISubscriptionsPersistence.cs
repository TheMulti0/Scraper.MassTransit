using System.Collections.Generic;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public interface ISubscriptionsPersistence
    {
        IEnumerable<Subscription> Get();
        
        void AddOrUpdate(Subscription subscription);

        void Remove(Subscription subscription);
    }
}