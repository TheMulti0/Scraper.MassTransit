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
                .AddLogging()
                .AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>()
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