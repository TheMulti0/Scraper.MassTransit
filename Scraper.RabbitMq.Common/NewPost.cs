using Scraper.Net;

namespace Scraper.RabbitMq.Common
{
    public record NewPost
    {
        public Post Post { get; init; }
        public string Platform { get; init; }
    }
}