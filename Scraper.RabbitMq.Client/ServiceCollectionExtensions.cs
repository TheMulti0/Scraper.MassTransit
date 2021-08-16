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
        /// <summary>
        /// Exposes a <see cref="INewPostSubscriptionsClient"/> and <see cref="IScraperService"/> all powered powered by RabbitMQ (using MassTransit)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <typeparam name="TNewPostsConsumer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddScraperRabbitMqClient<TNewPostsConsumer>(
            this IServiceCollection services,
            RabbitMqConfig config = null) where TNewPostsConsumer : class, IConsumer<NewPost>
        {
            return services.AddScraperRabbitMqClient(typeof(TNewPostsConsumer), config);
        }

        /// <summary>
        /// Exposes a <see cref="INewPostSubscriptionsClient"/> and <see cref="IScraperService"/> all powered powered by RabbitMQ (using MassTransit)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="newPostsConsumer"></param>
        /// <param name="config"></param>
        /// <typeparam name="TConsumer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddScraperRabbitMqClient(
            this IServiceCollection services,
            Type newPostsConsumer = null,
            RabbitMqConfig config = null)
        {
            return services
                .AddMassTransit(
                    x =>
                    {
                        if (newPostsConsumer != null)
                        {
                            x.AddConsumer(newPostsConsumer);
                        }
                        
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
                .AddSingleton<INewPostSubscriptionsClient, NewPostSubscriptionsClient>()
                .AddSingleton<IScraperService, ScraperRabbitMqClient>();
        }
    }
}