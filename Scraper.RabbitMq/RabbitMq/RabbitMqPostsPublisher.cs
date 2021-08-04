using System;
using System.Text.Json;
using RabbitMQ.Client;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class RabbitMqPostsPublisher : IPostsPublisher, IDisposable
    {
        private readonly IModel _channel;

        public RabbitMqPostsPublisher(IModel channel)
        {
            _channel = channel;

            DeclareExchange();
        }

        private void DeclareExchange()
        {
            _channel.ExchangeDeclare(
                exchange: RabbitMqConstants.PipeName,
                type: ExchangeType.Fanout,
                durable: true);
        }

        public void Send(Post post, string platform)
        {
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(post);
            
            _channel.BasicPublish(
                exchange: RabbitMqConstants.PipeName,
                routingKey: platform,
                body: json);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}