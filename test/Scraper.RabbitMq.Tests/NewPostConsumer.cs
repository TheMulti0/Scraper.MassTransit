using System.Threading.Tasks;
using MassTransit;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Tests
{
    internal class NewPostConsumer : IConsumer<NewPost>
    {
        public Task Consume(ConsumeContext<NewPost> context)
        {
            return Task.CompletedTask;
        }
    }
}