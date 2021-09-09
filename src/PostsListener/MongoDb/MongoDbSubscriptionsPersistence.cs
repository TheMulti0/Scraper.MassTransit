using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class MongoDbSubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly IMongoCollection<Subscription> _subscriptions;
        private readonly ILogger<MongoDbSubscriptionsPersistence> _logger;
        private readonly UpdateOptions _updateOptions;

        public MongoDbSubscriptionsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbSubscriptionsPersistence> logger)
        {
            _logger = logger;
            _subscriptions = database.GetCollection<Subscription>(nameof(Subscription));
            
            _updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
        }

        public IEnumerable<Subscription> Get() => _subscriptions.AsQueryable();

        public void AddOrUpdate(Subscription subscription)
        {
            var result = _subscriptions.UpdateOne(
                s => s.Platform == subscription.Platform && s.Id == subscription.Id,
                Builders<Subscription>.Update
                    .Set(post => post.PollInterval, subscription.PollInterval),
                _updateOptions);

            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to add or update subscription");
            }
            
            _logger.LogInformation("Updated subscription [{}] {} {}", subscription.Platform, subscription.Id, subscription.PollInterval);
        }

        public void Remove(Subscription subscription)
        {
            Subscription rhs = subscription;
            
            var result = _subscriptions.DeleteOne(lhs => lhs.Platform == rhs.Platform && lhs.Id == rhs.Id);
            if (!result.IsAcknowledged)
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            _logger.LogInformation("Removed subscription [{}] {}", subscription.Platform, subscription.Id);
        }
    }
}