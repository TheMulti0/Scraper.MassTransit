using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Scraper.MassTransit.Client;
using Scraper.Net;
using Scraper.MassTransit.Common;
using Scraper.Net.Stream;

namespace PostsListener
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddStream(services);
            AddMassTransit(services);
            AddPersistence(services);
            
            services.AddSingleton<LastPostFilter>();
            services.AddSingleton<PostUrlFilter>();
            services.AddSingleton<PostFilter>();
            services.AddSingleton<ISubscriptionsManager, SubscriptionsManager>();
            services.AddHostedService<SubscriptionsService>();
        }

        private void AddStream(IServiceCollection services)
        {
            var config = _configuration.GetSection("PostsStreamer").Get<PostsStreamerConfig>() ?? new PostsStreamerConfig();

            services.AddStream(
                provider => provider.GetRequiredService<PostFilter>().Filter,
                config);
            
            var streamerManagerConfig = _configuration.GetSection("StreamerManager").Get<StreamerManagerConfig>() ?? new StreamerManagerConfig();
            
            services.AddSingleton(
                provider => new StreamerManager(
                    streamerManagerConfig,
                    provider.GetRequiredService<PostsStreamer>(),
                    provider.GetRequiredService<IBus>(),
                    provider.GetRequiredService<ILogger<StreamerManager>>()));
        }

        private void AddMassTransit(IServiceCollection services)
        {
            services
                .AddMassTransit(
                    x =>
                    {
                        x.AddScraperMassTransitClient();
                        
                        x.AddConsumer<AddOrUpdateNewPostSubscriptionConsumer>();
                        x.AddConsumer<RemoveNewPostSubscriptionConsumer>();
                        x.AddConsumer<GetNewPostSubscriptionsConsumer>();

                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                var rabbitMqConfig = _configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
                                
                                cfg.Host(rabbitMqConfig.ConnectionString);
                                
                                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                cfg.ConfigureEndpoints(context);
                            });
                    })
                .AddMassTransitHostedService();
        }

        private void AddPersistence(IServiceCollection services)
        {
            IConfigurationSection mongoDbConfigg = _configuration.GetSection("MongoDb");
            var mongoDbConfig = mongoDbConfigg.Get<MongoDbConfig>();
            if (mongoDbConfigg.GetValue<bool>("Enabled") && mongoDbConfig != null)
            {
                services.AddSingleton(MongoDatabaseFactory.CreateDatabase(mongoDbConfig));

                services.AddSingleton<ISubscriptionsPersistence>(
                    provider => new MongoDbSubscriptionsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        provider.GetRequiredService<ILogger<MongoDbSubscriptionsPersistence>>()));
                
                services.AddSingleton<ILastPostsPersistence>(
                    provider => new MongoDbLastPostsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        provider.GetRequiredService<ILogger<MongoDbLastPostsPersistence>>()));
                
                services.AddSingleton<IPostUrlsPersistence>(
                    provider => new MongoDbPostUrlsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        mongoDbConfigg.GetSection("PostUrls").Get<PostUrlsPersistenceConfig>(),
                        provider.GetRequiredService<ILogger<MongoDbPostUrlsPersistence>>()));
            }
            else
            {
                services.AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>();
                services.AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>();
                services.AddSingleton<IPostUrlsPersistence, InMemoryPostUrlsPersistence>();
            }
        }
    }
}