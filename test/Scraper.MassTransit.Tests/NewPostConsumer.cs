using System.Threading.Tasks;
using MassTransit;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Tests
{
    internal class NewPostConsumer : IConsumer<NewPost>
    {
        public Task Consume(ConsumeContext<NewPost> context)
        {
            return Task.CompletedTask;
        }
    }
}