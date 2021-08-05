using RabbitMQ.Client;

namespace Scraper.RabbitMq.Common
{
    public static class PostsExchangeFactory
    {
        public static void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(
                exchange: RabbitMqConstants.PipeName,
                type: ExchangeType.Fanout,
                durable: true);
        }
    }
}