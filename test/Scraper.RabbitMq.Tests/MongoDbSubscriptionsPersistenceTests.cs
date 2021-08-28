using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class MongoDbSubscriptionsPersistenceTests
    {
        private readonly CrudTestBase<Subscription> _crud;

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
            
            _crud = new CrudTestBase<Subscription>(
                () => new Subscription
                {
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