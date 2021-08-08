using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace Scraper.RabbitMq.Tests
{
    [TestClass]
    public class MongoDbPostUrlsPersistenceTests
    {
        private readonly IPostUrlsPersistence _persistence;

        public MongoDbPostUrlsPersistenceTests()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<IPostUrlsPersistence>(_ =>
                {
                    IMongoDatabase mongoDatabase = MongoDatabaseFactory.CreateDatabase(
                        new MongoDbConfig
                        {
                            DatabaseName = "ScraperDb"
                        });
                    var config = new PostUrlsPersistenceConfig();
                    
                    return new MongoDbPostUrlsPersistence(mongoDatabase, config);
                })
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