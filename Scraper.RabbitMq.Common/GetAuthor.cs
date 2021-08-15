namespace Scraper.RabbitMq.Common
{
    public record GetAuthor
    {
        public string Id { get; init; }

        public string Platform { get; init; }
    }
}