using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    internal class ScrapedPostsService
    {
        private readonly ConcurrentDictionary<Guid, Channel<Post>> _posts = new();
        private readonly ILogger<ScrapedPostsService> _logger;

        public ScrapedPostsService(ILogger<ScrapedPostsService> logger)
        {
            _logger = logger;
        }

        public async Task AddPostAsync(Guid requestId, Post post, CancellationToken ct)
        {
            if (!_posts.ContainsKey(requestId))
            {
                _logger.LogInformation("Received post {} for not found request id {}", post.Url, requestId);
                return;
            }
            
            await _posts[requestId].Writer.WriteAsync(post, ct);
        }
        
        public IAsyncEnumerable<Post> GetPostsAsync(Guid requestId)
        {
            return GetOrAdd(requestId).Reader.ReadAllAsync();
        }
        
        public void Complete(Guid requestId)
        {
            _posts[requestId].Writer.Complete();
            
            _posts.TryRemove(requestId, out var _);
        }
        
        private Channel<Post> GetOrAdd(Guid requestId)
        {
            return _posts.GetOrAdd(
                requestId,
                _ => Channel.CreateUnbounded<Post>());
        }
    }
}