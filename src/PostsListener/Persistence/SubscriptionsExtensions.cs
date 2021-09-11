using Scraper.MassTransit.Common;

namespace PostsListener
{
    public static class SubscriptionsExtensions
    {
        public static Subscription ToSubscription(this SubscriptionEntity entity)
        {
            return new Subscription
            {
                Id = entity.Id,
                Platform = entity.Platform,
                PollInterval = entity.PollInterval
            };
        }
        
        public static SubscriptionEntity ToEntity(this Subscription entity)
        {
            return new SubscriptionEntity
            {
                Id = entity.Id,
                Platform = entity.Platform,
                PollInterval = entity.PollInterval
            };
        }
    }
}