using System;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Net;

namespace PostsListener
{
    public class LastPostFilter
    {
        private readonly ILastPostsPersistence _persistence;

        public LastPostFilter(ILastPostsPersistence persistence)
        {
            _persistence = persistence;
        }

        public async Task<bool> FilterAsync(
            Post post,
            string platform,
            TimeSpan toleration,
            CancellationToken ct)
        {
            if (post.CreationDate == null)
            {
                return false;
            }

            LastPost existing = await _persistence.GetAsync(platform, post.Author.Id, ct);

            DateTime? lastPostCreationDate = existing?.LastPostTime.Floor(toleration);
            DateTime postCreationDate = ((DateTime)post.CreationDate).Floor(toleration);
            // post.CreationDate cannot be null here

            if (lastPostCreationDate >= postCreationDate)
            {
                return false;
            }

            await _persistence.AddOrUpdateAsync(platform, post.Author.Id, (DateTime)post.CreationDate, ct);
            return true;
        }
    }
}