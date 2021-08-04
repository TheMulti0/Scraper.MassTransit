using System;
using System.Text.Json;
using RabbitMQ.Client;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class RabbitMqPostsPublisher : IPostsPublisher, IDisposable
    {
        private const string ExchangeName = "posts";
        
        private readonly IModel _channel;

        public RabbitMqPostsPublisher(IModel channel)
        {
            _channel = channel;

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Fanout,
                durable: true);
        }

        public void Send(Post post, string platform)
        {
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(post);
            
            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: platform,
                body: json);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}