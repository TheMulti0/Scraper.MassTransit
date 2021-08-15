using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Scraper.Net;

namespace Scraper.RabbitMq.Client
{
    internal class ScrapedPostsService
    {
        private readonly Subject<(Guid RequestId, Post Post)> _posts = new();
        
        public void NewPost(Guid requestId, Post post)
        {
            _posts.OnNext((requestId, post));
        }

        public IAsyncEnumerable<Post> AwaitPosts(Guid requestId)
        {
            return _posts
                .Where(tuple => tuple.RequestId == requestId)
                .Select(tuple => tuple.Post)
                .ToAsyncEnumerable();
        }
    }
}