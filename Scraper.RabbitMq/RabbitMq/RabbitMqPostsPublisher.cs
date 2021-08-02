using System;
using System.Text.Json;
using RabbitMQ.Client;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class RabbitMqPostsPublisher : IPostsPublisher, IDisposable
    {
        private const string Exchange = "posts";
        
        private readonly IModel _channel;

        public RabbitMqPostsPublisher(IModel channel)
        {
            _channel = channel;

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