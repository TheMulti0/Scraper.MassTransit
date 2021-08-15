using System;
using System.Threading.Tasks;
using System.Web;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class AddOrUpdateSubscriptionConsumer : IConsumer<AddOrUpdateSubscription>
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;
        private readonly ILogger<AddOrUpdateSubscriptionConsumer> _logger;

        public AddOrUpdateSubscriptionConsumer(
            ISubscriptionsManager subscriptionsManager,
            ISubscriptionsPersistence subscriptionsPersistence,
            ILogger<AddOrUpdateSubscriptionConsumer> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _subscriptionsPersistence = subscriptionsPersistence;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AddOrUpdateSubscription> context)
        {
            AddOrUpdateSubscription request = context.Message;

            string id = request.Id;
            string platform = request.Platform;
            TimeSpan pollInterval = request.PollInterval;
            
            if (pollInterval <= TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(pollInterval));
            }
            
            var subscription = new Subscription
            {
                Platform = platform,
                Id = HttpUtility.UrlDecode(id),
                PollInterval = pollInterval
            };
            
            _subscriptionsManager.AddOrUpdate(subscription);
            _subscriptionsPersistence.AddOrUpdate(subscription);
            
            _logger.LogInformation("Subscribed to [{}] {} with interval of {}", platform, id, pollInterval);

            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}