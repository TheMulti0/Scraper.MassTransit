using System.Threading.Tasks;

namespace Scraper.RabbitMq
{
    public interface IPostUrlsPersistence
    {
        Task<bool> ExistsAsync(string url);

        Task AddAsync(string url);

        Task RemoveAsync(string url);
    }
}