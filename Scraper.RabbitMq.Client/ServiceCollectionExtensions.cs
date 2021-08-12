using System;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
                .AddSingleton<ISubscriptionsClient>(_ => new SubscriptionsRestClient(serverUri));
        }
    }
}