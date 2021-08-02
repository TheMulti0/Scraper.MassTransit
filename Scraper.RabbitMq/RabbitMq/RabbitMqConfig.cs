using System;

namespace Scraper.RabbitMq
{
    public class RabbitMqConfig
    {
        public Uri ConnectionString { get; set; } = new("amqp://guest:guest@localhost:5672//");
    }
}