using System;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class PostFilter
    {
        private const string Facebook = "facebook";
        private readonly LastPostFilter _lastPostFilter;
        private readonly PostUrlFilter _postUrlFilter;

        public PostFilter(
            LastPostFilter lastPostFilter,
            PostUrlFilter postUrlFilter)
        {
            _lastPostFilter = lastPostFilter;
            _postUrlFilter = postUrlFilter;
        }

        public bool Filter(Post post, string platform)
        {
            if (platform == Facebook)
            {
                return _lastPostFilter.Filter(post, platform, TimeSpan.FromMinutes(1)) &&
                       _postUrlFilter.Filter(post);
            }
            
            return _lastPostFilter.Filter(post, platform, TimeSpan.Zero);
        }
    }
}