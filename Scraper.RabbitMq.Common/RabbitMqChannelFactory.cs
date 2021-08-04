using RabbitMQ.Client;

namespace Scraper.RabbitMq.Common
{
    public static class RabbitMqChannelFactory
    {
        public static IModel Create(RabbitMqConfig config)
        {
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString
            };
                
            return factory
                .CreateConnection()
                .CreateModel();
        }
    }
}