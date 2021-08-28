using System;

namespace Scraper.RabbitMq.Common
{
    public record AddOrUpdateNewPostSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }
    }
}