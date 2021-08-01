using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class PostsConsumer
    {
        private readonly string _platform;
        private readonly string _id;
        
        public PostsConsumer(Subscription subscription)
        {
            _platform = subscription.Platform;
            _id = subscription.Id;
        }

        public void OnPost(Post post)
        {
            // Push to rabbitmq
        }
    }
}