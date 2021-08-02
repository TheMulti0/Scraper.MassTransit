using Scraper.Net;

namespace Scraper.RabbitMq
{
    internal class PostsConsumer
    {
        private readonly string _platform;
        private readonly PostsPublisher _publisher;
        
        public PostsConsumer(
            Subscription subscription,
            PostsPublisher publisher)
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