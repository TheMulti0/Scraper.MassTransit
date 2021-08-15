using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Scraper.Net;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScraperRabbitMqClient<TConsumer>(
            this IServiceCollection services,
            Uri serverUri,
            RabbitMqConfig config = null) where TConsumer : class, IConsumer<PostReceived>
        {
            return services
                .AddMassTransit(
                    x =>
                    {
                        x.AddConsumer<TConsumer>();
                        x.AddConsumer<ScrapedPostsConsumer>();
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            config ??= new RabbitMqConfig();
                            
                            cfg.Host(config.ConnectionString);
                            
                            cfg.ConfigureJsonSerializer(settings => new JsonSerializerSettings
                            {
                                Converters =
                                {
                                    new PostJsonConverter()
                                }
                            });
                            
                            cfg.ConfigureEndpoints(context);
                        });
                    })
                .AddMassTransitHostedService()
                .AddSingleton<ScrapedPostsService>()
                .AddSingleton<ISubscriptionsClient>(_ => new SubscriptionsRestClient(serverUri))
                .AddSingleton<IScraperService>(provider => ActivatorUtilities.CreateInstance<ScraperRabbitMqClient>(provider));
        }
    }
}