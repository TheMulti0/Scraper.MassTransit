using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Scraper.MassTransit.Common;

namespace PostsListener
{
    public class PollNewPostSubscriptionConsumer : IConsumer<PollNewPostSubscription>
    {
        private readonly StreamerManager _streamerManager;
        private readonly ILogger<PollNewPostSubscriptionConsumer> _logger;

        public PollNewPostSubscriptionConsumer(
            StreamerManager streamerManager,
            ILogger<PollNewPostSubscriptionConsumer> logger)
        {
            _streamerManager = streamerManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PollNewPostSubscription> context)
        {
            PollNewPostSubscription request = context.Message;
            string id = request.Id;
            string platform = request.Platform;

            (Subscription _, PostSubscription subscription) = _streamerManager.Get()
                .First(pair => pair.Key.Id == id && pair.Key.Platform == platform);
            
            _logger.LogInformation("Triggering poll for [{}] {}", platform, id);
            
            subscription.TriggerPoll();
            
            await context.RespondAsync(OperationSucceeded.Instance);
        }
    }
}