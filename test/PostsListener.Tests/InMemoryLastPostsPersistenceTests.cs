using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostsListener.Tests
{
    [TestClass]
    public class InMemoryLastPostsPersistenceTests
    {
        private readonly CrudTestBase<LastPost> _crud;

        public InMemoryLastPostsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddLogging()
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
                subscriptionsPersistence.GetAsync,
                (post, ct) => subscriptionsPersistence.AddOrUpdateAsync(post.Platform, post.AuthorId, post.LastPostTime, ct),
                subscriptionsPersistence.RemoveAsync);
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
    }
}