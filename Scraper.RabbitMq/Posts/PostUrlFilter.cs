using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class PostUrlFilter
    {
        private readonly IPostUrlsPersistence _persistence;

        public PostUrlFilter(IPostUrlsPersistence persistence)
        {
            _persistence = persistence;
        }

        public bool Filter(Post post)
        {
            return post.Url != null && _persistence.Exists(post.Url);
        } 
    }
}