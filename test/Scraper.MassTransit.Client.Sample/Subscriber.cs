using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Scraper.MassTransit.Client.Sample
{
    internal class Subscriber : BackgroundService
    {
        private readonly INewPostSubscriptionsClient _subscriptionsClient;
        private readonly ILogger<Subscriber> _logger;

        public Subscriber(
            INewPostSubscriptionsClient subscriptionsClient,
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
                await _subscriptionsClient.AddOrUpdateSubscription(id, platform, TimeSpan.FromDays(1), stoppingToken);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to subscribe to [{}] {}", platform, id);   
            }

            Console.WriteLine("Press enter to unsubscribe");
            Console.ReadLine();
            
            try
            {
                await _subscriptionsClient.RemoveSubscription(id, platform, stoppingToken);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to unsubscribe from [{}] {}", platform, id);   
            }
        }
    }
}