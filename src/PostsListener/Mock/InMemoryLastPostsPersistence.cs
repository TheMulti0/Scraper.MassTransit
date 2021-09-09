using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PostsListener
{
    public class InMemoryLastPostsPersistence : ILastPostsPersistence
    {
        private readonly object _lastPostsLock = new();
        private readonly List<LastPost> _lastPosts = new();
        private readonly ILogger<InMemoryLastPostsPersistence> _logger;

        public InMemoryLastPostsPersistence(ILogger<InMemoryLastPostsPersistence> logger)
        {
            _logger = logger;
        }

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

            _logger.LogInformation("Updated [{}] {} last post time to {}", platform, authorId, lastPostTime);
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
            
            _logger.LogInformation("Removed [{}] {} last post time", lastPost.Platform, lastPost.Id);
        }
    }
}