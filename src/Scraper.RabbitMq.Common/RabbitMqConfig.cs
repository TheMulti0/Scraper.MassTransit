using System;

namespace Scraper.RabbitMq.Common
{
    public class RabbitMqConfig
    {
        public Uri ConnectionString { get; set; } = new("amqp://guest:guest@localhost:5672//");
    }
}