using System;
using System.Linq;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class LastPostFilter
    {
        private readonly ILastPostsPersistence _persistence;
        private readonly TimeSpan _toleration = TimeSpan.FromMinutes(1);

        public LastPostFilter(ILastPostsPersistence persistence)
        {
            _persistence = persistence;
        }

        public bool Filter(Post post, string platform)
        {
            if (post.CreationDate == null)
            {
                return false;
            }

            LastPost existing = _persistence.Get(platform, post.AuthorId);

            DateTime? lastPostCreationDate = existing?.LastPostTime.Floor(_toleration);
            DateTime postCreationDate = ((DateTime) post.CreationDate).Floor(_toleration); // post.CreationDate cannot be null here

            if (lastPostCreationDate >= postCreationDate)
            {
                return false;
            }

            _persistence.AddOrUpdate(platform, post.AuthorId, (DateTime) post.CreationDate);
            
            return true;
        } 
    }
}