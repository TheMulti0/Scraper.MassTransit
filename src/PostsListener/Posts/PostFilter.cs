using System;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Net;

namespace PostsListener
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

        public async Task<bool> FilterAsync(
            Post post,
            string platform,
            CancellationToken ct)
        {
            if (platform == Facebook)
            {
                return await _lastPostFilter.FilterAsync(post, platform, TimeSpan.FromMinutes(1), ct) &&
                       await _postUrlFilter.FilterAsync(post, ct);
            }

            return await _lastPostFilter.FilterAsync(post, platform, TimeSpan.Zero, ct);
        }
    }
}