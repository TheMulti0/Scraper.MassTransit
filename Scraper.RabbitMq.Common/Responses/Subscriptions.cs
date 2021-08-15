using System.Collections.Generic;

namespace Scraper.RabbitMq.Common
{
    public record Subscriptions
    {
        public IEnumerable<Subscription> Items { get; init; }
    }
}