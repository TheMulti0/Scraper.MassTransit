using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TheMulti0.Console;

namespace Scraper.MassTransit.Client.Sample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddTheMulti0Console())
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services
                            .AddScraperRabbitMqClient<NewPostConsumer>()
                            .AddHostedService<Subscriber>()
                            .AddHostedService<Scraper>();
                    });
    }
}