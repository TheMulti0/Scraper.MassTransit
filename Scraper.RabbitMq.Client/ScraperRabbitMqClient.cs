using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public class ScraperRabbitMqClient : IScraperRabbitMqClient
    {
        private readonly INewPostsConsumer _newPostsConsumer;
        private readonly ISubscriptionsClient _subscriptionsClient;

        public IObservable<NewPost> NewPosts => _newPostsConsumer.NewPosts;

        public ScraperRabbitMqClient(
            INewPostsConsumer newPostsConsumer,
            ISubscriptionsClient subscriptionsClient)
        {
            _newPostsConsumer = newPostsConsumer;
            _subscriptionsClient = subscriptionsClient;
        }

        public Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct = default)
        {
            return _subscriptionsClient.GetSubscriptionsAsync(ct);
        }

        public Task SubscribeAsync(
            string platform,
            string id,
            TimeSpan pollInterval,
            CancellationToken ct = default)
        {
            return _subscriptionsClient.SubscribeAsync(platform, id, pollInterval, ct);
        }

        public Task UnsubscribeAsync(
            string platform,
            string id,
            CancellationToken ct = default)
        {
            return _subscriptionsClient.UnsubscribeAsync(platform, id, ct);
        }
    }
}