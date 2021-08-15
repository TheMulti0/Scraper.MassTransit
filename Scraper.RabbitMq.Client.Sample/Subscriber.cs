using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Scraper.RabbitMq.Client.Sample
{
    internal class Subscriber : BackgroundService
    {
        private readonly ISubscriptionsClient _subscriptionsClient;
        private readonly ILogger<Subscriber> _logger;

        public Subscriber(
            ISubscriptionsClient subscriptionsClient,
            ILogger<Subscriber> logger)
        {
            _subscriptionsClient = subscriptionsClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string platform = "facebook";
            const string id = "NaftaliBennett";

            try
            {
                await _subscriptionsClient.SubscribeAsync(platform, id, TimeSpan.FromDays(1), stoppingToken);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to subscribe to [{}] {}", platform, id);   
            }

            Console.WriteLine("Press enter to unsubscribe");
            Console.ReadLine();
            
            try
            {
                await _subscriptionsClient.UnsubscribeAsync(platform, id, stoppingToken);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to unsubscribe from [{}] {}", platform, id);   
            }
        }
    }
}