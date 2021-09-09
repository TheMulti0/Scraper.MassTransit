using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostsListener.Tests
{
    [TestClass]
    public class MongoDbLastPostsPersistenceTests
    {
        private readonly CrudTestBase<LastPost> _crud;
        private readonly ILastPostsPersistence _subscriptionsPersistence;

        public MongoDbLastPostsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILastPostsPersistence>(
                    _ => new MongoDbLastPostsPersistence(
                        MongoDatabaseFactory.CreateDatabase(new MongoDbConfig
                        {
                            DatabaseName = "ScraperDb"
                        }),
                        NullLogger<MongoDbLastPostsPersistence>.Instance))
                .BuildServiceProvider();

            _subscriptionsPersistence = provider.GetRequiredService<ILastPostsPersistence>();
            
            _crud = new CrudTestBase<LastPost>(
                () => new LastPost
                {
                    Platform = "facebook",
                    AuthorId = "test",
                    LastPostTime = DateTime.Now
                },
                _subscriptionsPersistence.Get,
                post => _subscriptionsPersistence.AddOrUpdate(post.Platform, post.AuthorId, post.LastPostTime),
                _subscriptionsPersistence.Remove);
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

        [TestMethod]
        public void TestUpdate()
        {
            _crud.Clear();

            const string authorId = "test";
            const string platform = "platform";

            var initialTime = DateTime.UnixEpoch;
            var updatedTime = initialTime.AddDays(1);

            _subscriptionsPersistence.AddOrUpdate(platform, authorId, initialTime);
            _subscriptionsPersistence.AddOrUpdate(platform, authorId, updatedTime);
            var item = _subscriptionsPersistence.Get().First(post => post.Platform == platform && post.AuthorId == authorId);
            
            Assert.AreEqual(updatedTime, item.LastPostTime);
        }
    }
}