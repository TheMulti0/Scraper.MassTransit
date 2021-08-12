using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TheMulti0.Console;

namespace Scraper.RabbitMq.Client.Sample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddTheMulti0Console())
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddScraperRabbitMqClient<PostReceivedConsumer>(new Uri("http://localhost:5000"))
                        .AddHostedService<Subscriber>();
                });
    }
}
