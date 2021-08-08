using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Scraper.RabbitMq
{
    public class MongoDbPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly IMongoCollection<SentPost> _collection;

        public MongoDbPostUrlsPersistence(
            IMongoDatabase database,
            PostUrlsPersistenceConfig config)
        {
            _collection = database.GetCollection<SentPost>("PostUrls");

            if (config.ExpirationTime > TimeSpan.Zero &&
                _collection.Indexes.List().ToList().Count < 2) // There shouldn't be more than two indices in the collection
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
       
        public Task<bool> ExistsAsync(string url)
        {
            return _collection
                .AsQueryable()
                .AnyAsync(sentUpdate => sentUpdate.Url == url);
        }

        public async Task AddAsync(string url)
        {
            var sentUpdate = new SentPost
            {
                SentAt = DateTime.Now,
                Url = url
            };
            
            await _collection.InsertOneAsync(sentUpdate);
        }

        public async Task RemoveAsync(string url)
        {
            await _collection.DeleteOneAsync(
                new FilterDefinitionBuilder<SentPost>()
                    .Eq(s => s.Url, url));
        }
    }
}