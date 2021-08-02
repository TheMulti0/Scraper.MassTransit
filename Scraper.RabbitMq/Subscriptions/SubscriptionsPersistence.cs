using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public interface ISubscriptionsPersistence
    {
        IEnumerable<Subscription> Get();
        
        void Add(Subscription subscription);

        void Remove(Subscription subscription);
    }
}