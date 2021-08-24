using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;

namespace Scraper.RabbitMq.Client
{
    internal class ScrapedPostConsumer : IConsumer<Post>
    {
        private readonly ScrapedPostsService _scrapedPostsService;

        public ScrapedPostConsumer(ScrapedPostsService scrapedPostsService)
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