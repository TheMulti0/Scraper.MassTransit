using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Scraper.MassTransit.Common;

namespace PostsListener.Tests
{
    [TestClass]
    public class MongoDbSubscriptionsPersistenceTests
    {
        private readonly CrudTestBase<SubscriptionEntity> _crud;

        public MongoDbSubscriptionsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ISubscriptionsPersistence>(
                    _ => new MongoDbSubscriptionsPersistence(
                        MongoDatabaseFactory.CreateDatabase(new MongoDbConfig
                        {
                            DatabaseName = "ScraperDb"
                        }),
                        NullLogger<MongoDbSubscriptionsPersistence>.Instance))
                .BuildServiceProvider();

            var subscriptionsPersistence = provider.GetRequiredService<ISubscriptionsPersistence>();
            
            var id = ObjectId.GenerateNewId();
            
            _crud = new CrudTestBase<SubscriptionEntity>(
                () => new SubscriptionEntity
                {
                    SubscriptionId = id,
                    Platform = "facebook",
                    Id = "test",
                    PollInterval = TimeSpan.FromHours(1)
                },
                subscriptionsPersistence.Get,
                subscriptionsPersistence.AddOrUpdate,
                subscriptionsPersistence.Remove);
        }
        
        [TestMethod]
        public void TestAddSingle()
        {
            _crud.TestAddSingle();
        }
        
        [TestMethod]
        public void TestAddRemoveSingle()
        {
            _crud.TestAddRemoveSingle();
        }
    }
}