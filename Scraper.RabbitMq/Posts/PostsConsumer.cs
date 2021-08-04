using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    internal class PostsConsumer
    {
        private readonly string _platform;
        private readonly IPostsPublisher _publisher;
        
        public PostsConsumer(
            Subscription subscription,
            IPostsPublisher publisher)
        {
            _platform = subscription.Platform;
            _publisher = publisher;
        }

        public void OnPost(Post post)
        {
            _publisher.Send(post, _platform);
        }
    }
}