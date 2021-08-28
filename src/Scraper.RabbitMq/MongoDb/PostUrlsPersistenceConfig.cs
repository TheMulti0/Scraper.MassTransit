using System;

namespace Scraper.RabbitMq
{
    public class PostUrlsPersistenceConfig
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromDays(1);
    }
}