using System;
using Newtonsoft.Json;
using Scraper.Net;

namespace Scraper.RabbitMq.Common
{
    internal class MediaJsonConverter : JsonConverter<IMediaItem>
    {
        private readonly JsonSerializer _serializer = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        public override void WriteJson(JsonWriter writer, IMediaItem value, JsonSerializer serializer)
        {
            _serializer.Serialize(writer, value);
        }

        public override IMediaItem ReadJson(
            JsonReader reader,
            Type objectType,
            IMediaItem existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return _serializer.Deserialize<IMediaItem>(reader);
        }
    }
}