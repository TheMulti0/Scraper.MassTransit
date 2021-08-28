namespace Scraper.RabbitMq.Common
{
    public record OperationStarted
    {
        public static OperationStarted Instance { get; } = new();
    }
}