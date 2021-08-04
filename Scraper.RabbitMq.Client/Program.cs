using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scraper.RabbitMq.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            new ServiceCollection()
                .AddLogging(builder => builder.AddConsole());
        }
    }
}