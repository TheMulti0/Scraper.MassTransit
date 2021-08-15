using System;
using System.Collections.Generic;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public interface ISubscriptionsManager
    {
        IDictionary<Subscription, IDisposable> Get();
        
        void AddOrUpdate(Subscription subscription);

        void Remove(Subscription subscription);
    }
}