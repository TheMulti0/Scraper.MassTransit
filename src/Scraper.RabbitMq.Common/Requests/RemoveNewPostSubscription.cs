namespace Scraper.RabbitMq.Common
{
    public record RemoveNewPostSubscription
    {
        public string Platform { get; init; }

        public string Id { get; init; }
    }
}