using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Scraper.RabbitMq
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionsManager _subscriptionsManager;

        public SubscriptionsController(ISubscriptionsManager subscriptionsManager)
        {
            _subscriptionsManager = subscriptionsManager;
        }

        [HttpGet]
        public IEnumerable<Subscription> Get()
        {
            return _subscriptionsManager.Get().Keys;
        }

        [HttpPost("{platform}/{id}")]
        public void Add(string platform, string id, TimeSpan pollInterval)
        {
            if (pollInterval <= TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(pollInterval));
            }

            var subscription = new Subscription
            {
                Platform = platform,
                Id = id,
                PollInterval = pollInterval
            };
            
            _subscriptionsManager.Add(subscription);
            
            // Persist to db
        }

        [HttpDelete("{platform}/{id}")]
        public void Remove(string platform, string id)
        {
            var subscription = new Subscription
            {
                Platform = platform,
                Id = id
            };
            
            _subscriptionsManager.Remove(subscription);
            
            // Persist to db
        }
    }
}