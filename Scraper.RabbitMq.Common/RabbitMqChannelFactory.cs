using RabbitMQ.Client;

namespace Scraper.RabbitMq.Common
{
    public static class RabbitMqChannelFactory
    {
        public static IModel Create(RabbitMqConfig config)
        {
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString,
                DispatchConsumersAsync = true,
                ConsumerDispatchConcurrency = config.ConcurrencyLevel
            };
                
            return factory
                .CreateConnection()
                .CreateModel();
        }
    }
}