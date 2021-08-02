using System;
using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public interface ILastPostsPersistence
    {
        IEnumerable<LastPost> Get();
        
        void AddOrUpdate(string platform, string authorId, DateTime lastPostTime);

        void Remove(LastPost lastPost);
    }
}