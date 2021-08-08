using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.RabbitMq
{
    public class InMemoryLastPostsPersistence : ILastPostsPersistence
    {
        private readonly object _lastPostsLock = new();
        private readonly List<LastPost> _lastPosts = new();
        
        public IEnumerable<LastPost> Get()
        {
            lock (_lastPostsLock)
            {
                return _lastPosts.ToArray();
            }
        }

        public LastPost Get(string platform, string authorId)
        {
            lock (_lastPostsLock)
            {
                return _lastPosts
                    .FirstOrDefault(lastPost => lastPost.Platform == platform && lastPost.AuthorId == authorId);    
            }
        }

        private void Add(LastPost lastPost)
        {
            lock (_lastPostsLock)
            {
                _lastPosts.Add(lastPost);
            }
        }

        public void AddOrUpdate(string platform, string authorId, DateTime lastPostTime)
        {
            lock (_lastPostsLock)
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
        }

        public void Remove(LastPost lastPost)
        {
            lock (_lastPostsLock)
            {
                if (!_lastPosts.Remove(lastPost))
                {
                    throw new InvalidOperationException("Failed to remove last post");
                }
            }
        }
    }
}