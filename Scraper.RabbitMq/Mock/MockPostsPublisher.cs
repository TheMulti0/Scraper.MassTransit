using Microsoft.Extensions.Logging;
using Scraper.Net;

namespace Scraper.RabbitMq
{
    public class MockPostsPublisher : IPostsPublisher
    {
        private readonly ILogger<MockPostsPublisher> _logger;

        public MockPostsPublisher(ILogger<MockPostsPublisher> logger)
        {
            _logger = logger;
        }

        public void Send(Post post, string platform)
        {
            _logger.LogInformation("Publishing post {} from platform {}", post, platform);
        }
    }
}