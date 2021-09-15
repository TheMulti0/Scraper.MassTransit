using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scraper.MassTransit.Common;

namespace PostsListener.Tests
{
    [TestFixture]
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
                _ => subscriptionsManager.Get().ToAsyncEnumerable(),
                (s, ct) => subscriptionsManager.AddOrUpdateAsync(s, ct: ct),
                subscriptionsManager.RemoveAsync);
        }
        
        [Test]
        public async Task TestAddSingleAsync()
        {
            await _crud.TestAddSingleAsync();
        }
        
        [Test]
        public async Task TestAddRemoveSingleAsync()
        {
            await _crud.TestAddRemoveSingleAsync();
        }
    }
}