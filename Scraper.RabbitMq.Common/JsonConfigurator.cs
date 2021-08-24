using Newtonsoft.Json;

namespace Scraper.RabbitMq.Common
{
    public static class JsonConfigurator
    {
        public static JsonSerializerSettings Configure(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new MediaJsonConverter());
            return settings;
        }
    }
}