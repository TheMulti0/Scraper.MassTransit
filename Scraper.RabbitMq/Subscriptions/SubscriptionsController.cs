using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(
            ISubscriptionsManager subscriptionsManager,
            ISubscriptionsPersistence subscriptionsPersistence,
            ILogger<SubscriptionsController> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _subscriptionsPersistence = subscriptionsPersistence;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Subscription> Get()
        {
            return _subscriptionsManager.Get().Keys;
        }

        [HttpPost("{platform}/{id}")]
        public void Add(string platform, string id, [FromForm] TimeSpan pollInterval)
        {
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
            
            _subscriptionsManager.Add(subscription);
            _subscriptionsPersistence.Add(subscription);
            
            _logger.LogInformation("Subscribed to [{}] {} with interval of {}", platform, id, pollInterval);
        }

        [HttpDelete("{platform}/{id}")]
        public void Remove(string platform, string id)
        {
            var subscription = new Subscription
            {
                Platform = platform,
                Id = HttpUtility.UrlDecode(id)
            };
            
            _subscriptionsManager.Remove(subscription);
            _subscriptionsPersistence.Remove(subscription);
            
            _logger.LogInformation("Unsubscribed to [{}] {}", platform, id);
        }
    }
}