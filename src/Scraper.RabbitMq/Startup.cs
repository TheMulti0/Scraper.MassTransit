using HtmlCssToImage.Net;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Scraper.Net;
using Scraper.Net.Facebook;
using Scraper.Net.Feeds;
using Scraper.Net.Screenshot;
using Scraper.Net.Stream;
using Scraper.Net.Twitter;
using Scraper.Net.YoutubeDl;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
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
            services.AddScraper(BuildScraper);
            
            AddStream(services);
            AddRabbitMq(services);
            AddPersistence(services);
            
            services.AddSingleton<LastPostFilter>();
            services.AddSingleton<PostUrlFilter>();
            services.AddSingleton<PostFilter>();
            services.AddSingleton<StreamerManager>();
            services.AddSingleton<ISubscriptionsManager, SubscriptionsManager>();
            services.AddHostedService<SubscriptionsService>();
            
            services.AddMassTransitHostedService();
        }

        private void AddStream(IServiceCollection services)
        {
            var config = _configuration.GetSection("PostsStreamer").Get<PostsStreamerConfig>() ?? new PostsStreamerConfig();

            services.AddStream(
                provider => provider.GetRequiredService<PostFilter>().Filter,
                config);
        }

        private void BuildScraper(ScraperBuilder builder)
        {
            IConfiguration scraperConfig = _configuration.GetSection("Scraper");
            
            IConfigurationSection feedsConfig = scraperConfig.GetSection("Feeds");
            if (feedsConfig.GetValue<bool?>("Enabled") != false)
            {
                builder.AddFeeds();
            }

            IConfigurationSection twitterConfig = scraperConfig.GetSection("Twitter");
            var twitterConfigg = twitterConfig.Get<TwitterConfig>();
            if (twitterConfig.GetValue<bool>("Enabled") && twitterConfigg != null)
            {
                builder.AddTwitter(twitterConfigg);
            }

            IConfigurationSection facebookConfig = scraperConfig.GetSection("Facebook");
            if (facebookConfig.GetValue<bool>("Enabled"))
            {
                builder.AddFacebook(facebookConfig.Get<FacebookConfig>());
            }

            IConfigurationSection youtubeDlConfig = scraperConfig.GetSection("YoutubeDl");
            if (youtubeDlConfig.GetValue<bool>("Enabled"))
            {
                builder.AddYoutubeDl(youtubeDlConfig.Get<YoutubeDlConfig>());
            }
            
            IConfigurationSection screenshotDlConfig = scraperConfig.GetSection("Screenshot");
            if (screenshotDlConfig.GetValue<bool>("Enabled"))
            {
                builder.AddScreenshot(
                    b => b.AddTwitter(),
                    screenshotDlConfig.Get<HtmlCssToImageCredentials>());
            }
        }

        private void AddRabbitMq(IServiceCollection services)
        {
            IConfigurationSection rabbitMqConfigg = _configuration.GetSection("RabbitMq");
            var rabbitMqConfig = rabbitMqConfigg.Get<RabbitMqConfig>();

            services.AddMassTransit(
                x =>
                {
                    x.AddConsumer<AddOrUpdateNewPostSubscriptionConsumer>();
                    x.AddConsumer<RemoveNewPostSubscriptionConsumer>();
                    x.AddConsumer<GetNewPostSubscriptionsConsumer>();
                    x.AddConsumer<GetAuthorConsumer>();
                    x.AddConsumer<GetPostsConsumer>();

                    if (rabbitMqConfigg.GetValue<bool>("Enabled") && rabbitMqConfig != null)
                    {
                        x.UsingRabbitMq(
                            (context, cfg) =>
                            {
                                cfg.Host(rabbitMqConfig.ConnectionString);
                                
                                cfg.ConfigureInterfaceJsonSerialization(typeof(IMediaItem));
                                
                                cfg.ConfigureEndpoints(context);
                            });
                    }
                    else
                    {
                        x.UsingInMemory((context, cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                        });
                    }
                });
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