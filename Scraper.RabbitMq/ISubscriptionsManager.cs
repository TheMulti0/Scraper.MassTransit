using System;
using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public interface ISubscriptionsManager
    {
        IDictionary<Subscription, IDisposable> Get();
        
        void Add(Subscription subscription);

        void Remove(Subscription subscription);
    }
}