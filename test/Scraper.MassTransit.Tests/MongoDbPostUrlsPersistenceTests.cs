using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace Scraper.MassTransit.Tests
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
                    
                    return new MongoDbPostUrlsPersistence(mongoDatabase, config, NullLogger<MongoDbPostUrlsPersistence>.Instance);
                })
                .BuildServiceProvider();

            _persistence = provider.GetRequiredService<IPostUrlsPersistence>();
        }
        
        [TestMethod]
        public void TestAddSingle()
        {
            const string url = "my-url";

            if (_persistence.Exists(url))
            {
                _persistence.Remove(url);
            }

            _persistence.Add(url);
            Assert.IsTrue(_persistence.Exists(url));
        }
        
        [TestMethod]
        public void TestAddRemoveSingle()
        {
            const string url = "my-url";

            if (_persistence.Exists(url))
            {
                _persistence.Remove(url);
            }

            _persistence.Add(url);
            Assert.IsTrue(_persistence.Exists(url));

            _persistence.Remove(url);
            Assert.IsFalse(_persistence.Exists(url));
        }
    }
}