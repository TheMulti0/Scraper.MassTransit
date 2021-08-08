using System;
using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public interface ILastPostsPersistence
    {
        IEnumerable<LastPost> Get();

        LastPost Get(string platform, string authorId);
        
        void AddOrUpdate(string platform, string authorId, DateTime lastPostTime);

        void Remove(LastPost lastPost);
    }
}