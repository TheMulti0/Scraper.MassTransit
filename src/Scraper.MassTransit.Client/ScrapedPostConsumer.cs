using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    internal class ScrapedPostConsumer : IConsumer<Post>
    {
        private readonly ScrapedPostsService _scrapedPostsService;

        public ScrapedPostConsumer(ScrapedPostsService scrapedPostsService)
        {
            _scrapedPostsService = scrapedPostsService;
        }

        public async Task Consume(ConsumeContext<Post> context)
        {
            await _scrapedPostsService.AddPostAsync(
                context.RequestId.Value,
                context.Message,
                context.CancellationToken);
        }
    }
}