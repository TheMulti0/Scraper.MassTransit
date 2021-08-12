using System;
using Newtonsoft.Json;
using Scraper.Net;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Scraper.RabbitMq.Common
{
    // TODO Directly use complete System.Text.Json serialization with MassTransit
    public class PostJsonConverter : JsonConverter<Post>
    {
        public override Post ReadJson(
            JsonReader reader,
            Type objectType,
            Post existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Post>(
                reader.ReadAsString()!);
        }

        public override void WriteJson(JsonWriter writer, Post value, JsonSerializer serializer)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(value);
            
            writer.WriteRawValue(json);
        }
    }
}