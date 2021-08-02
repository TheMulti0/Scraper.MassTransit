using System;
using System.Linq;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class LastPostFilter
    {
        private readonly ILastPostsPersistence _persistence;

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
            
            LastPost existing = _persistence.Get()
                .FirstOrDefault(lastPost => lastPost.Platform == platform && lastPost.AuthorId == post.AuthorId);

            if (existing != null && existing.LastPostTime >= post.CreationDate)
            {
                return false;
            }
            
            _persistence.AddOrUpdate(platform, post.AuthorId, (DateTime) post.CreationDate);
            
            return true;
        } 
    }
}