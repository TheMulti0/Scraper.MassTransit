using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class GetPostsConsumer : IConsumer<GetPosts>
    {
        private readonly IScraperService _scraperService;

        public GetPostsConsumer(IScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        public async Task Consume(ConsumeContext<GetPosts> context)
        {
            GetPosts request = context.Message;
            CancellationToken ct = context.CancellationToken;
            
            IAsyncEnumerable<Post> posts = _scraperService.GetPostsAsync(
                request.Id,
                request.Platform,
                ct);

            await foreach (Post post in posts.WithCancellation(ct))
            {
                await context.Publish(
                    post,
                    publishContext => publishContext.RequestId = context.RequestId, 
                    ct);
            }
            
            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}