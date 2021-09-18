﻿using System;
using System.Linq;
using System.Threading;
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
        private Func<Subscription> _factory;
        private Func<Subscription, CancellationToken, Task> _add;

        public SubscriptionsManagerTests()
        {
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();
            new Startup(config).ConfigureServices(services);
            ServiceProvider provider = services.AddLogging().BuildServiceProvider();

            var subscriptionsManager = provider.GetRequiredService<ISubscriptionsManager>();

            _factory = () => new Subscription
            {
                Platform = "facebook",
                Id = "test",
                PollInterval = TimeSpan.FromHours(1)
            };
            _add = (s, ct) => subscriptionsManager.AddOrUpdateAsync(s, ct: ct);
            
            _crud = new CrudTestBase<Subscription>(
                _factory,
                _ => subscriptionsManager.Get().ToAsyncEnumerable(),
                _add,
                subscriptionsManager.RemoveAsync);
        }
        
        [Test]
        public async Task TestAddSingleAsync()
        {
            await _crud.TestAddSingleAsync();
        }

        [Test]
        public async Task TestUpdateSingleAsync()
        {
            await _crud.ClearAsync();

            Subscription subscription = _factory();
            await _add(subscription, default);
            
            Subscription newSubscription = subscription with { PollInterval = subscription.PollInterval + TimeSpan.FromSeconds(1) };
            await _add(
                newSubscription,
                default);
        }
        
        [Test]
        public async Task TestAddRemoveSingleAsync()
        {
            await _crud.TestAddRemoveSingleAsync();
        }
    }
}