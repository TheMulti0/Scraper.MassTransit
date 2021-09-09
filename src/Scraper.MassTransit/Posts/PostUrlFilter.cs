using Scraper.Net;

namespace Scraper.MassTransit
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
            if (post.Url == null || _persistence.Exists(post.Url))
            {
                return false;
            }
            
            _persistence.Add(post.Url);
                
            return true;
        } 
    }
}