using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class InMemoryLastPostsPersistenceTests
    {
        private readonly CrudTestBase<LastPost> _crud;

        public InMemoryLastPostsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>()
                .BuildServiceProvider();

            var subscriptionsPersistence = provider.GetRequiredService<ILastPostsPersistence>();
            
            _crud = new CrudTestBase<LastPost>(
                () => new LastPost
                {
                    Platform = "facebook",
                    AuthorId = "test",
                    LastPostTime = DateTime.Now
                },
                subscriptionsPersistence.Get,
                post => subscriptionsPersistence.AddOrUpdate(post.Platform, post.AuthorId, post.LastPostTime),
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