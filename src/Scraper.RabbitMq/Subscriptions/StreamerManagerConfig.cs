using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public class StreamerManagerConfig
    {
        public Dictionary<string, int> PlatformMultipliers { get; set; } = new();
    }
}