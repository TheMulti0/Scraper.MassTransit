using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class RabbitMqPostsPublisher : IPostsPublisher, IDisposable
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqPostsPublisher> _logger;

        public RabbitMqPostsPublisher(
            IModel channel,
            ILogger<RabbitMqPostsPublisher> logger)
        {
            _channel = channel;
            _logger = logger;

            PostsExchangeFactory.DeclareExchange(channel);
        }

        public void Send(Post post, string platform)
        {
            _logger.LogInformation("Sending post {}", post.Url);
            
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