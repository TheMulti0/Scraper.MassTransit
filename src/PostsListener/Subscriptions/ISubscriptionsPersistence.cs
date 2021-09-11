using System.Collections.Generic;

namespace PostsListener
{
    public interface ISubscriptionsPersistence
    {
        IEnumerable<SubscriptionEntity> Get();
        
        SubscriptionEntity Get(string id, string platform);
        
        void AddOrUpdate(SubscriptionEntity subscription);

        void Remove(SubscriptionEntity subscription);
    }
}