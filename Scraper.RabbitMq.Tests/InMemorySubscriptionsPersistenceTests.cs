using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class InMemorySubscriptionsPersistenceTests
    {
        private readonly CrudTestBase<Subscription> _crud;

        public InMemorySubscriptionsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>()
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
                subscriptionsPersistence.Add,
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