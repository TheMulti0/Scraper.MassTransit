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
    internal class ScrapedPostsManager
    {
        private readonly ConcurrentDictionary<Guid, Subject<Post>> _posts = new();
        private readonly ILogger<ScrapedPostsManager> _logger;

        public ScrapedPostsManager(ILogger<ScrapedPostsManager> logger)
        {
            _logger = logger;
        }

        public void AddPost(Guid requestId, Post post)
        {
            if (!_posts.ContainsKey(requestId))
            {
                _logger.LogInformation("Received post {} for not found request id {}", post.Url, requestId);
                return;
            }
            
            _posts[requestId].OnNext(post);
        }
        
        public IObservable<Post> GetPostsAsync(Guid requestId)
        {
            return _posts.GetOrAdd(
                requestId,
                _ => new Subject<Post>());
        }
        
        public void OnComplete(Guid requestId)
        {
            _posts.TryRemove(requestId, out Subject<Post> _);
        }
    }
}