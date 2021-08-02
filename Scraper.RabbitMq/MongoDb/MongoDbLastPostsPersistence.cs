using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Scraper.RabbitMq
{
    public class MongoDbLastPostsPersistence : ILastPostsPersistence
    {
        private readonly IMongoCollection<LastPost> _lastPosts;
        private readonly UpdateOptions _updateOptions;

        public MongoDbLastPostsPersistence(IMongoDatabase database)
        {
            _lastPosts = database.GetCollection<LastPost>(nameof(LastPost));
            
            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IEnumerable<LastPost> Get() => _lastPosts.AsQueryable();

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
        }
    }
}