using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheMulti0.Console;

namespace Scraper.RabbitMq
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    builder => builder
                        .AddTheMulti0Console()
                        .AddSentry())
                .ConfigureServices(
                    (context, services) => new Startup(context.Configuration).ConfigureServices(services));
        }
    }
}