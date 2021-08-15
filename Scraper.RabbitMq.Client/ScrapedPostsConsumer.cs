using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;

namespace Scraper.RabbitMq.Client
{
    internal class ScrapedPostsConsumer : IConsumer<Post>
    {
        private readonly ScrapedPostsService _scrapedPostsService;

        public ScrapedPostsConsumer(ScrapedPostsService scrapedPostsService)
        {
            _scrapedPostsService = scrapedPostsService;
        }

        public Task Consume(ConsumeContext<Post> context)
        {
            _scrapedPostsService.NewPost(context.RequestId.Value, context.Message);

            return Task.CompletedTask;
        }
    }
}