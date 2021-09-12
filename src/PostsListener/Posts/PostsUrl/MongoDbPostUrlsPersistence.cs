using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace PostsListener
{
    public class MongoDbPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly IMongoCollection<SentPost> _collection;
        private readonly ILogger<MongoDbPostUrlsPersistence> _logger;

        public MongoDbPostUrlsPersistence(
            IMongoDatabase database,
            PostUrlsPersistenceConfig config,
            ILogger<MongoDbPostUrlsPersistence> logger)
        {
            _logger = logger;
            _collection = database.GetCollection<SentPost>("PostUrls");

            if (config.ExpirationTime > TimeSpan.Zero &&
                _collection.Indexes.List()
                    .ToList()
                    .Count < 2) // There shouldn't be more than two indices in the collection
            {
                CreateExpirationIndex(config);
            }
        }

        private void CreateExpirationIndex(PostUrlsPersistenceConfig config)
        {
            IndexKeysDefinition<SentPost> keys = Builders<SentPost>.IndexKeys
                .Ascending(update => update.SentAt);

            var options = new CreateIndexOptions
            {
                ExpireAfter = config.ExpirationTime
            };

            var indexModel = new CreateIndexModel<SentPost>(keys, options);

            _collection.Indexes.CreateOne(indexModel);
        }

        public bool Exists(string url)
        {
            return _collection
                .AsQueryable()
                .Any(sentUpdate => sentUpdate.Url == url);
        }

        public void Add(string url)
        {
            var sentUpdate = new SentPost
            {
                SentAt = DateTime.Now,
                Url = url
            };

            _collection.InsertOne(sentUpdate);

            _logger.LogInformation("Added post {}", url);
        }

        public void Remove(string url)
        {
            _collection.DeleteOne(
                new FilterDefinitionBuilder<SentPost>()
                    .Eq(s => s.Url, url));

            _logger.LogInformation("Removed post {}", url);
        }
    }
}