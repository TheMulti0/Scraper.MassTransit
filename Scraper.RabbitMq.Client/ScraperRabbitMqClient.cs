using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    internal class ScraperRabbitMqClient : IScraperService
    {
        private readonly IRequestClient<GetAuthor> _getAuthor;
        private readonly IRequestClient<GetPosts> _getPosts;
        private readonly ScrapedPostsService _scrapedPostsService;

        public ScraperRabbitMqClient(IBus bus, ScrapedPostsService scrapedPostsService)
        {
            _scrapedPostsService = scrapedPostsService;
            _getAuthor = bus.CreateRequestClient<GetAuthor>();
            _getPosts = bus.CreateRequestClient<GetPosts>();
        }

        public async Task<Author> GetAuthorAsync(string id, string platform, CancellationToken ct = default)
        {
            var request = new GetAuthor
            {
                Id = id,
                Platform = platform
            };

            Response<Author> response = await _getAuthor.GetResponse<Author>(request, ct);

            return response.Message;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(string id, string platform, CancellationToken ct = default)
        {
            var request = new GetPosts
            {
                Id = id,
                Platform = platform
            };

            RequestHandle<GetPosts> requestHandle = _getPosts.Create(request, ct);

            IAsyncEnumerable<Post> posts = GetPosts(requestHandle);
            await foreach (Post post in posts.WithCancellation(ct))
            {
                yield return post;
            }
        }

        private IAsyncEnumerable<Post> GetPosts(RequestHandle requestHandle)
        {
            var completeSignal = Observable.FromAsync(
                () => requestHandle.GetResponse<OperationSucceeded>());
            
            return _scrapedPostsService
                .GetPosts(requestHandle.RequestId)
                .TakeUntil(completeSignal)
                .ToAsyncEnumerable();
        }
    }
}