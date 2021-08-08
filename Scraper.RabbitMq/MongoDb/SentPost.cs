using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Scraper.RabbitMq
{
    internal class SentPost
    {
        [BsonId]
        public string Url { get; set; }

        public DateTime SentAt { get; set; }
    }
}