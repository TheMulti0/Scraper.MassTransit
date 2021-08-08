using HtmlCssToImage.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
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
            services.AddControllers();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "Scraper.RabbitMq",
                            Version = "v1"
                        });
                });
            
            services.AddScraper(BuildScraper);
            
            AddStream(services);
            AddRabbitMq(services);
            AddPersistence(services);
            
            services.AddSingleton<LastPostFilter>();
            services.AddSingleton<PostFilter>();
            services.AddSingleton<ISubscriptionsManager, SubscriptionsManager>();
            services.AddHostedService<SubscriptionsService>();
        }

        private void AddStream(IServiceCollection services)
        {
            var config = _configuration.GetSection("PostsStreamer").Get<PostsStreamerConfig>();

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
            if (rabbitMqConfigg.GetValue<bool>("Enabled") && rabbitMqConfig != null)
            {
                services.AddSingleton<IPostsPublisher>(
                    provider => new RabbitMqPostsPublisher(
                        RabbitMqChannelFactory.Create(rabbitMqConfig),
                        provider.GetRequiredService<ILogger<RabbitMqPostsPublisher>>()));
            }
            else
            {
                services.AddSingleton<IPostsPublisher, MockPostsPublisher>();
            }
        }

        private void AddPersistence(IServiceCollection services)
        {
            IConfigurationSection mongoDbConfigg = _configuration.GetSection("MongoDb");
            var mongoDbConfig = mongoDbConfigg.Get<MongoDbConfig>();
            if (mongoDbConfigg.GetValue<bool>("Enabled") && mongoDbConfig != null)
            {
                services.AddSingleton(MongoDatabaseFactory.CreateDatabase(mongoDbConfig));

                services.AddSingleton<ISubscriptionsPersistence>(
                    provider => new MongoDbSubscriptionsPersistence(provider.GetRequiredService<IMongoDatabase>()));
                
                services.AddSingleton<ILastPostsPersistence>(
                    provider => new MongoDbLastPostsPersistence(
                        provider.GetRequiredService<IMongoDatabase>(),
                        provider.GetRequiredService<ILogger<MongoDbLastPostsPersistence>>()));
            }
            else
            {
                services.AddSingleton<ISubscriptionsPersistence, InMemorySubscriptionsPersistence>();
                services.AddSingleton<ILastPostsPersistence, InMemoryLastPostsPersistence>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scraper.Rest v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}