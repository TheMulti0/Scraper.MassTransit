using HtmlCssToImage.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scraper.Net;
using Scraper.Net.Facebook;
using Scraper.Net.Feeds;
using Scraper.Net.Screenshot;
using Scraper.Net.Stream;
using Scraper.Net.Twitter;
using Scraper.Net.YoutubeDl;

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

            services.AddScraper(BuildScraper).AddStream((post, platform) => true);

            services.AddSingleton<ISubscriptionsManager, InMemorySubscriptionsManager>();
            services.AddHostedService<SubscriptionsService>();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scraper.Rest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}