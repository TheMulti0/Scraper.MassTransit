using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using TheMulti0.Console;

namespace Scraper.RabbitMq
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    builder => builder
                        .AddTheMulti0Console(
                            options => options.IncludeThreadIds = true))
                .ConfigureWebHostDefaults(
                    builder => builder
                        .UseSentry()
                        .UseStartup<Startup>());
    }
}