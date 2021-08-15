namespace Scraper.RabbitMq.Common
{
    public record OperationSucceeded
    {
        public static OperationSucceeded Instance { get; } = new();
    }
}