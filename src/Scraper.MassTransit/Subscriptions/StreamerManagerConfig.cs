using System.Collections.Generic;

namespace Scraper.MassTransit
{
    public class StreamerManagerConfig
    {
        public Dictionary<string, int> PlatformMultipliers { get; set; } = new();
    }
}