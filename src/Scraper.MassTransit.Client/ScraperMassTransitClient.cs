using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using async_enumerable_dotnet;
using MassTransit;
using Scraper.Net;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Client
{
    internal class ScraperMassTransitClient : IScraperService
    {
        private readonly IRequestClient<GetAuthor> _getAuthor;
        private readonly IRequestClient<GetPosts> _getPosts;
        private readonly ScrapedPostsService _scrapedPostsService;

        public ScraperMassTransitClient(IBus bus, ScrapedPostsService scrapedPostsService)
        {
            _scrapedPostsService = scrapedPostsService;
            _getAuthor = bus.CreateRequestClient<GetAuthor>();
            _getPosts = bus.CreateRequestClient<GetPosts>();
        }

        public async Task<Author> GetAuthorAsync(
            string id,
            string platform,
            CancellationToken ct = default)
        {
            var request = new GetAuthor
            {
                Id = id,
                Platform = platform
            };

            Response<Author> response = await _getAuthor.GetResponse<Author>(request, ct);

            return response.Message;
        }

        public async IAsyncEnumerable<Post> GetPostsAsync(
            string id,
            string platform,
            [EnumeratorCancellation] CancellationToken ct = default)
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

            _scrapedPostsService.Complete(requestHandle.RequestId);
        }

        private IAsyncEnumerable<Post> GetPosts(RequestHandle requestHandle)
        {
            var completeSignal = async_enumerable_dotnet.AsyncEnumerable.FromTask(
                requestHandle.GetResponse<OperationSucceeded>());
            
            return _scrapedPostsService
                .GetPostsAsync(requestHandle.RequestId)
                .TakeUntil(completeSignal);
        }
    }
}