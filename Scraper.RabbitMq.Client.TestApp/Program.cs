using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.RabbitMq.Client.TestApp
{
    internal class Program
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

            await scraperRabbitMqClient.SubscribeAsync(platform, id, TimeSpan.FromMinutes(1));

            scraperRabbitMqClient.NewPosts.Subscribe(
                post =>
                {
                    Console.WriteLine(post);
                });

            await Task.Delay(TimeSpan.FromMinutes(1));

            await scraperRabbitMqClient.UnsubscribeAsync(platform, id);
        }
    }
}