using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.MassTransit.Common;

namespace PostsListener.Tests
{
    [TestClass]
    public class SubscriptionsManagerTests
    {
        private readonly CrudTestBase<Subscription> _crud;

        public SubscriptionsManagerTests()
        {
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();
            new Startup(config).ConfigureServices(services);
            ServiceProvider provider = services.AddLogging().BuildServiceProvider();

            var subscriptionsManager = provider.GetRequiredService<ISubscriptionsManager>();
            
            _crud = new CrudTestBase<Subscription>(
                () => new Subscription
                {
                    Platform = "facebook",
                    Id = "test",
                    PollInterval = TimeSpan.FromHours(1)
                },
                () => subscriptionsManager.Get().Keys,
                subscriptionsManager.AddOrUpdate,
                subscriptionsManager.Remove);
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