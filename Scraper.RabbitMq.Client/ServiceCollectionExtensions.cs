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
            Uri rabbitMqConnectionString = null)
        {
            return services
                .AddSingleton<INewPostsConsumer>(
                    provider =>
                    {
                        var config = new RabbitMqConfig();
                        if (rabbitMqConnectionString != null)
                        {
                            config.ConnectionString = rabbitMqConnectionString;
                        }

                        IModel channel = RabbitMqChannelFactory.Create(config);

                        return new RabbitMqPostsConsumer(channel);
                    })
                .AddSingleton<ISubscriptionsClient>(_ => new SubscriptionsRestClient(serverUri))
                .AddSingleton<IScraperRabbitMqClient, ScraperRabbitMqClient>();
        }
    }
}