using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.RabbitMq;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class SubscriptionsControllerTests
    {
        private readonly SubscriptionsController _subscriptionsController;
        private readonly CrudTestBase<Subscription> _crud;

        public SubscriptionsControllerTests()
        {
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();
            new Startup(config).ConfigureServices(services);
            ServiceProvider provider = services.BuildServiceProvider();

            _subscriptionsController = new SubscriptionsController(provider.GetRequiredService<ISubscriptionsManager>());

            _crud = new CrudTestBase<Subscription>(
                () => new Subscription
                {
                    Platform = "facebook",
                    Id = "test",
                    PollInterval = TimeSpan.FromHours(1)
                },
                () => _subscriptionsController.Get(),
                subscription =>
                    _subscriptionsController.Add(subscription.Platform, subscription.Id, subscription.PollInterval),
                subscription => _subscriptionsController.Remove(subscription.Platform, subscription.Id));
        }
        
        [TestMethod]
        public void TestAddWithIllegalInterval()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _subscriptionsController.Add("", "", TimeSpan.Zero));
            Assert.ThrowsException<ArgumentNullException>(() => _subscriptionsController.Add("", "", TimeSpan.FromSeconds(-1)));
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