using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    internal class ScrapedPostConsumer : IConsumer<Post>
    {
        private readonly ScrapedPostsManager _scrapedPostsManager;

        public ScrapedPostConsumer(ScrapedPostsManager scrapedPostsManager)
        {
            _scrapedPostsManager = scrapedPostsManager;
        }

        public Task Consume(ConsumeContext<Post> context)
        {
            _scrapedPostsManager.AddPost(
                context.RequestId.Value,
                context.Message);

            return Task.CompletedTask;
        }
    }
}