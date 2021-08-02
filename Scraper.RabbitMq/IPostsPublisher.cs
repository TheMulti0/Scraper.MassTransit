using Scraper.Net;

namespace Scraper.RabbitMq
{
    public interface IPostsPublisher
    {
        void Send(Post post, string platform);
    }
}