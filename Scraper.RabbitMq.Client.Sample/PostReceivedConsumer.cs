using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client.Sample
{
    internal class PostReceivedConsumer : IConsumer<PostReceived>
    {
        private readonly ILogger<PostReceivedConsumer> _logger;

        public PostReceivedConsumer(ILogger<PostReceivedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PostReceived> context)
        {
            _logger.LogInformation(context.Message.Post.Url);

            return Task.CompletedTask;
        }
    }
}