using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class InMemoryPostUrlsPersistenceTests
    {
        private readonly IPostUrlsPersistence _persistence;

        public InMemoryPostUrlsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>()
                .BuildServiceProvider();

            _persistence = provider.GetRequiredService<IPostUrlsPersistence>();
        }
        
        [TestMethod]
        public async Task TestAddSingle()
        {
            const string url = "my-url";

            if (await _persistence.ExistsAsync(url))
            {
                await _persistence.RemoveAsync(url);
            }

            await _persistence.AddAsync(url);
            Assert.IsTrue(await _persistence.ExistsAsync(url));
        }
        
        [TestMethod]
        public async Task TestAddRemoveSingle()
        {
            const string url = "my-url";

            if (await _persistence.ExistsAsync(url))
            {
                await _persistence.RemoveAsync(url);
            }

            await _persistence.AddAsync(url);
            Assert.IsTrue(await _persistence.ExistsAsync(url));

            await _persistence.RemoveAsync(url);
            Assert.IsFalse(await _persistence.ExistsAsync(url));
        }
    }
}