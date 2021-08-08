using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class MongoDbSubscriptionsPersistence : ISubscriptionsPersistence
    {
        private readonly IMongoCollection<Subscription> _subscriptions;
        private readonly ILogger<MongoDbSubscriptionsPersistence> _logger;

        public MongoDbSubscriptionsPersistence(
            IMongoDatabase database,
            ILogger<MongoDbSubscriptionsPersistence> logger)
        {
            _logger = logger;
            _subscriptions = database.GetCollection<Subscription>(nameof(Subscription));
        }

        public IEnumerable<Subscription> Get() => _subscriptions.AsQueryable();

        public void Add(Subscription subscription)
        {
            _subscriptions.InsertOne(subscription);
            
            _logger.LogInformation("Added subscription [{}] {}", subscription.Platform, subscription.Id);
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