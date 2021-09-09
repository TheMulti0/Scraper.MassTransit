using Scraper.Net;

namespace Scraper.MassTransit.Common
{
    public record NewPost
    {
        public Post Post { get; init; }
        public string Platform { get; init; }
    }
}