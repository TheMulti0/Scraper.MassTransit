using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.RabbitMq
{
    public class InMemoryPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly object _postUrlsLock = new();
        private readonly List<string> _postUrls = new();

        public Task<bool> ExistsAsync(string url)
        {
            lock (_postUrlsLock)
            {
                return Task.FromResult(_postUrls.Contains(url));
            }
        }

        public Task AddAsync(string url)
        {
            lock (_postUrlsLock)
            {
                _postUrls.Add(url);
            }
            
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string url)
        {
            lock (_postUrlsLock)
            {
                return _postUrls.Remove(url)
                    ? Task.CompletedTask
                    : throw new InvalidOperationException($"Failed to remove url {url}");    
            }
        }
    }
}