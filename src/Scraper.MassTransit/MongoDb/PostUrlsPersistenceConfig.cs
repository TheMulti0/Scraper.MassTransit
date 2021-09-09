using System;

namespace Scraper.MassTransit
{
    public class PostUrlsPersistenceConfig
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromDays(1);
    }
}