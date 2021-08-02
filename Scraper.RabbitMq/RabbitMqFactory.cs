using RabbitMQ.Client;

namespace Scraper.RabbitMq
{
    public static class RabbitMqFactory
    {
        public static IModel Create(RabbitMqConfig config)
        {
            ConnectionFactory factory = CreateFactory(config);

            IConnection connection = factory.CreateConnection();

            return connection.CreateModel();
        }

        private static ConnectionFactory CreateFactory(RabbitMqConfig config)
        {
            var factory = new ConnectionFactory();

            if (config.HostName != null)
            {
                factory.HostName = config.HostName;
            }
            if (config.Port == 0)
            {
                factory.Port = config.Port;
            }
            if (config.UserName != null)
            {
                factory.UserName = config.UserName;
            }
            if (config.Password != null)
            {
                factory.Password = config.Password;
            }
            
            return factory;
        }
    }
}