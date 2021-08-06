using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScraperRabbitMqClient(
            this IServiceCollection services,
            Uri serverUri,
            RabbitMqConsumerConfig config = null)
        {
            return services
                .AddSingleton<INewPostsConsumer>(
                    _ =>
                    {
                        config ??= new RabbitMqConsumerConfig();
                        
                        IModel channel = RabbitMqChannelFactory.Create(config);

                        return new RabbitMqPostsConsumer(channel, config);
                    })
                .AddSingleton<ISubscriptionsClient>(_ => new SubscriptionsRestClient(serverUri))
                .AddSingleton<IScraperRabbitMqClient, ScraperRabbitMqClient>();
        }
    }
}