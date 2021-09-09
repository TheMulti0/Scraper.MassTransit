using System;
using System.Collections.Generic;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public interface ISubscriptionsManager
    {
        IDictionary<Subscription, IDisposable> Get();
        
        void AddOrUpdate(Subscription subscription);

        void Remove(Subscription subscription);
    }
}