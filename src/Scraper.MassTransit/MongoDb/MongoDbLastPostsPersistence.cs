using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Scraper.MassTransit
{
    public class MongoDbLastPostsPersistence : ILastPostsPersistence
    {
        private readonly ILogger<MongoDbLastPostsPersistence> _logger;
        private readonly IMongoCollection<LastPost> _lastPosts;
        private readonly UpdateOptions _updateOptions;

        public MongoDbLastPostsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbLastPostsPersistence> logger)
        {
            _logger = logger;
            _lastPosts = database.GetCollection<LastPost>(nameof(LastPost));
            
            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IEnumerable<LastPost> Get() => _lastPosts.AsQueryable();
        
        public LastPost Get(string platform, string authorId)
        {
            return _lastPosts
                .AsQueryable()
                .FirstOrDefault(lastPost => lastPost.Platform == platform && lastPost.AuthorId == authorId);
        }

        public void AddOrUpdate(string platform, string authorId, DateTime lastPostTime)
        {
            var result = _lastPosts.UpdateOne(
                post => post.Platform == platform && post.AuthorId == authorId,
                Builders<LastPost>.Update
                    .Set(post => post.LastPostTime, lastPostTime),
                _updateOptions);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to add or update last post");
            }
            
            _logger.LogInformation("Updated [{}] {} last post time to {}", platform, authorId, lastPostTime);
        }

        public void Remove(LastPost lastPost)
        {
            LastPost rhs = lastPost;
            
            DeleteResult result = _lastPosts
                .DeleteOne(lhs => lhs.Platform == rhs.Platform && lhs.AuthorId == rhs.AuthorId);
            
            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to remove last post");
            }
            
            _logger.LogInformation("Removed [{}] {} last post time", lastPost.Platform, lastPost.Id);
        }
    }
}