namespace Scraper.RabbitMq.Common
{
    public record GetPosts
    {
        public string Id { get; init; }

        public string Platform { get; init; }
    }
}