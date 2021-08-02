using System;
using System.Text.Json;
using RabbitMQ.Client;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class PostsPublisher : IDisposable
    {
        private const string Exchange = "posts";
        
        private readonly IModel _channel;

        public PostsPublisher(RabbitMqConfig config)
        {
            _channel = RabbitMqFactory.Create(config);

            _channel.ExchangeDeclare(
                exchange: Exchange,
                type: ExchangeType.Fanout,
                durable: true);
        }

        public void Send(Post post, string platform)
        {
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(post);
            
            _channel.BasicPublish(
                exchange: Exchange,
                routingKey: platform,
                body: json);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}