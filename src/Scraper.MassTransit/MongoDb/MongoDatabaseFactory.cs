using MongoDB.Driver;

namespace Scraper.MassTransit
{
    public static class MongoDatabaseFactory
    {
        public static IMongoDatabase CreateDatabase(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString.ToString());

            return client.GetDatabase(config.DatabaseName);
        }
    }
}