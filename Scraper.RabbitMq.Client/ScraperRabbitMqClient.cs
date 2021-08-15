using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Contracts.JobService;
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

            Response<OperationStarted> response = await _getPosts.GetResponse<OperationStarted>(request, ct);

            await foreach (Post post in _scrapedPostsService.AwaitPosts(response.RequestId.Value).WithCancellation(ct))
            {
                yield return post;
            }
        }
    }
}