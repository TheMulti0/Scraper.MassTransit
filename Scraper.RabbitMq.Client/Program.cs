using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var scraperRabbitMqClient = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddScraperRabbitMqClient(new Uri("http://localhost:5000"))
                .BuildServiceProvider()
                .GetRequiredService<IScraperRabbitMqClient>();

            const string platform = "feeds";
            const string id = "http://www.ynet.co.il/Integration/StoryRss2.xml";

            try
            {
                await scraperRabbitMqClient.SubscribeAsync(platform, id, TimeSpan.FromMinutes(1));
            }
            catch
            {
                
            }

            scraperRabbitMqClient.NewPosts.Subscribe(
                post =>
                {
                    Console.WriteLine(post);
                });

            await Task.Delay(TimeSpan.FromMinutes(1));

            await scraperRabbitMqClient.UnsubscribeAsync(platform, id);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScraperRabbitMqClient(
            this IServiceCollection services,
            Uri serverUri,
            RabbitMqConfig config = null)
        {
            return services
                .AddSingleton<INewPostsConsumer>(
                    provider =>
                    {
                        config ??= provider.GetService<RabbitMqConfig>() ??
                                   new RabbitMqConfig();

                        IModel channel = RabbitMqChannelFactory.Create(config);

                        return new RabbitMqPostsConsumer(channel);
                    })
                .AddSingleton<ISubscriptionsClient>(_ => new SubscriptionsRestClient(serverUri))
                .AddSingleton<IScraperRabbitMqClient, ScraperRabbitMqClient>();
        }
    }
}