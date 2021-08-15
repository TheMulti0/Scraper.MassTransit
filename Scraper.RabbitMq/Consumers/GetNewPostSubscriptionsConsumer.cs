﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class GetNewPostSubscriptionsConsumer : IConsumer<GetNewPostSubscriptions>
    {
        private readonly ISubscriptionsManager _subscriptionsManager;

        public GetNewPostSubscriptionsConsumer(ISubscriptionsManager subscriptionsManager)
        {
            _subscriptionsManager = subscriptionsManager;
        }

        public async Task Consume(ConsumeContext<GetNewPostSubscriptions> context)
        {
            ICollection<Subscription> subscriptions = _subscriptionsManager.Get().Keys;

            await context.RespondAsync(
                new Subscriptions
                {
                    Items = subscriptions
                });
        }
    }
}