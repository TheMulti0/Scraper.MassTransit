using System;
using System.Linq;
using System.Threading.Tasks;
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
                _subscriptionsPersistence.GetAsync,
                (post, ct) => _subscriptionsPersistence.AddOrUpdateAsync(post.Platform, post.AuthorId, post.LastPostTime, ct),
                _subscriptionsPersistence.RemoveAsync);
        }
        
        [TestMethod]
        public async Task TestAddSingleAsync()
        {
            await _crud.TestAddSingleAsync();
        }
        
        [TestMethod]
        public async Task TestAddRemoveSingleAsync()
        {
            await _crud.TestAddRemoveSingleAsync();
        }

        [TestMethod]
        public async Task TestUpdate()
        {
            await _crud.Clear();

            const string authorId = "test";
            const string platform = "platform";

            var initialTime = DateTime.UnixEpoch;
            var updatedTime = initialTime.AddDays(1);

            await _subscriptionsPersistence.AddOrUpdateAsync(platform, authorId, initialTime);
            await _subscriptionsPersistence.AddOrUpdateAsync(platform, authorId, updatedTime);
            var item = await _subscriptionsPersistence.GetAsync().FirstAsync(post => post.Platform == platform && post.AuthorId == authorId);
            
            Assert.AreEqual(updatedTime, item.LastPostTime);
        }
    }
}