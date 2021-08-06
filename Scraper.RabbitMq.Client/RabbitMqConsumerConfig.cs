using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public class RabbitMqConsumerConfig : RabbitMqConfig
    {
        public bool AutoAcknowledge { get; set; }

        public bool RejectRequeue { get; set; }
    }
}