using RabbitMQ.Client;

namespace Scraper.RabbitMq
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