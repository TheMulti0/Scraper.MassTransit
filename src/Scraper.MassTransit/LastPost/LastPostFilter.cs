using System;
using Scraper.Net;

namespace Scraper.MassTransit
{
    public class LastPostFilter
    {
        private readonly ILastPostsPersistence _persistence;

        public LastPostFilter(ILastPostsPersistence persistence)
        {
            _persistence = persistence;
        }

        public bool Filter(Post post, string platform, TimeSpan toleration)
        {
            if (post.CreationDate == null)
            {
                return false;
            }

            LastPost existing = _persistence.Get(platform, post.AuthorId);

            DateTime? lastPostCreationDate = existing?.LastPostTime.Floor(toleration);
            DateTime postCreationDate = ((DateTime) post.CreationDate).Floor(toleration); // post.CreationDate cannot be null here

            if (lastPostCreationDate >= postCreationDate)
            {
                return false;
            }

            _persistence.AddOrUpdate(platform, post.AuthorId, (DateTime) post.CreationDate);
            return true;
        } 
    }
}