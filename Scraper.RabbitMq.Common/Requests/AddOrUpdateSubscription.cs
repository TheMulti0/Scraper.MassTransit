using System;

namespace Scraper.RabbitMq.Common
{
    public record AddOrUpdateSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }
    }
}