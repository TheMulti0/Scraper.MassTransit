using System;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class PostFilter
    {
        private readonly LastPostFilter _filter;

        public PostFilter(
            LastPostFilter filter)
        {
            _filter = filter;
        }

        public bool Filter(Post post, string platform)
        {
            return _filter
                .Filter(
                    post,
                    platform,
                    platform == "facebook" 
                        ? TimeSpan.FromMinutes(1) 
                        : TimeSpan.Zero);
        }
    }
}