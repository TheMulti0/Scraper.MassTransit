using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

            IEnumerable<LastPost> lastPosts = _persistence.Get();
            LastPost existing = lastPosts
                .FirstOrDefault(lastPost => lastPost.Platform == platform && lastPost.AuthorId == post.AuthorId);

            if (existing == null)
            {
                Console.WriteLine(JsonSerializer.Serialize(lastPosts));
            }
            
            if (existing.LastPostTime >= post.CreationDate)
            {
                return false;
            }
            
            _persistence.AddOrUpdate(platform, post.AuthorId, (DateTime) post.CreationDate);
            
            return true;
        } 
    }
}