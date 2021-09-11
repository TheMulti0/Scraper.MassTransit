using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace PostsListener
{
    public class MongoDbSubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly IMongoCollection<SubscriptionEntity> _subscriptions;
        private readonly ILogger<MongoDbSubscriptionsPersistence> _logger;
        private readonly UpdateOptions _updateOptions;

        public MongoDbSubscriptionsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbSubscriptionsPersistence> logger)
        {
            _logger = logger;
            _subscriptions = database.GetCollection<SubscriptionEntity>(nameof(SubscriptionEntity));
            
            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IEnumerable<SubscriptionEntity> Get() => _subscriptions.AsQueryable();
        
        public SubscriptionEntity Get(string id, string platform)
        {
            return _subscriptions
                .AsQueryable()
                .Where(s => s.Id == id && s.Platform == platform)
                .FirstOrDefault();
        }
        
        private SubscriptionEntity Get(ObjectId id)
        {
            return _subscriptions
                .AsQueryable()
                .Where(s => s.SubscriptionId == id)
                .FirstOrDefault();
        }

        public void AddOrUpdate(SubscriptionEntity subscription)
        {
            UpdateResult result;
            do
            {
                var existing = Get(subscription.SubscriptionId);
                
                int version = existing?.Version ?? subscription.Version;

                UpdateDefinition<SubscriptionEntity> updateDefinition = Builders<SubscriptionEntity>.Update
                    .Set(s => s.Version, version++)
                    .Set(s => s.PollInterval, subscription.PollInterval)
                    .SetOnInsert(s => s.Id, subscription.Id)
                    .SetOnInsert(s => s.Platform, subscription.Platform);

                result = _subscriptions.UpdateOne(
                    s => s.SubscriptionId == subscription.SubscriptionId &&
                         s.Version == version,
                    updateDefinition,
                    _updateOptions);

                if (!result.IsAcknowledged)
                {
                    throw new InvalidOperationException("Failed to add or update subscription");
                }    
            }
            while (result.ModifiedCount < 1 &&
                   result.UpsertedId == BsonObjectId.Empty);

            _logger.LogInformation("Updated subscription [{}] {} {}", subscription.Platform, subscription.Id, subscription.PollInterval);
        }

        public void Remove(SubscriptionEntity subscription)
        {
            var result = _subscriptions
                .DeleteOne(s => s.SubscriptionId == subscription.SubscriptionId);
            
            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            _logger.LogInformation("Removed subscription [{}] {}", subscription.Platform, subscription.Id);
        }
    }
}