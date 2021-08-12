using System;
using System.Threading.Tasks;
using MassTransit;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client.Sample
{
    internal class PostReceivedConsumer : IConsumer<PostReceived>
    {
        public Task Consume(ConsumeContext<PostReceived> context)
        {
            Console.WriteLine(context.Message);

            return Task.CompletedTask;
        }
    }
}