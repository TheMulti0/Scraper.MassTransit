using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.RabbitMq
{
    public class InMemoryLastPostsPersistence : ILastPostsPersistence
    {
        private readonly ConcurrentBag<LastPost> _lastPosts = new();
        
        public IEnumerable<LastPost> Get() => _lastPosts;

        private void Add(LastPost lastPost) => _lastPosts.Add(lastPost);
        
        public void AddOrUpdate(string platform, string authorId, DateTime lastPostTime)
        {
            var lastPost = new LastPost
            {
                Platform = platform,
                AuthorId = authorId,
                LastPostTime = lastPostTime
            };

            if (_lastPosts.Contains(lastPost))
            {
                Remove(lastPost);
            }
            Add(lastPost);
        }

        public void Remove(LastPost lastPost)
        {
            if (!_lastPosts.TryTake(out lastPost))
            {
                throw new InvalidOperationException("Failed to remove last post");
            }
        }
    }
}