namespace Scraper.RabbitMq
{
    public interface IPostUrlsPersistence
    {
        bool Exists(string url);

        void Add(string url);

        void Remove(string url);
    }
}