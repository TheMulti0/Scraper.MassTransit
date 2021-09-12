using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace PostsListener
{
    public class InMemoryPostUrlsPersistence : IPostUrlsPersistence
    {
        private readonly object _postUrlsLock = new();
        private readonly List<string> _postUrls = new();
        private readonly ILogger<InMemoryPostUrlsPersistence> _logger;

        public InMemoryPostUrlsPersistence(ILogger<InMemoryPostUrlsPersistence> logger)
        {
            _logger = logger;
        }

        public bool Exists(string url)
        {
            lock (_postUrlsLock)
            {
                return _postUrls.Contains(url);
            }
        }

        public void Add(string url)
        {
            lock (_postUrlsLock)
            {
                _postUrls.Add(url);
            }

            _logger.LogInformation("Added post {}", url);
        }

        public void Remove(string url)
        {
            lock (_postUrlsLock)
            {
                if (!_postUrls.Remove(url))
                {
                    throw new InvalidOperationException($"Failed to remove url {url}");
                }
            }

            _logger.LogInformation("Removed post {}", url);
        }
    }
}