using System;
using System.Linq;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Net;
using Scraper.MassTransit.Common;

namespace Scraper.MassTransit.Client
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Exposes a <see cref="INewPostSubscriptionsClient"/> and <see cref="IScraperService"/> all powered powered by RabbitMQ (using MassTransit)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="interfacesToIncludeTypeNames"></param>
        /// <typeparam name="TNewPostConsumer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddScraperRabbitMqClient<TNewPostConsumer>(
            this IServiceCollection services,
            RabbitMqConfig config = null,
            params Type[] interfacesToIncludeTypeNames) where TNewPostConsumer : class, IConsumer<NewPost>
        {
            return services.AddScraperRabbitMqClient(typeof(TNewPostConsumer), config, interfacesToIncludeTypeNames);
        }

        /// <summary>
        /// Exposes a <see cref="INewPostSubscriptionsClient"/> and <see cref="IScraperService"/> all powered powered by RabbitMQ (using MassTransit)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="newPostConsumer"></param>
        /// <param name="config"></param>
        /// <param name="interfacesToIncludeTypeNames"></param>
        /// <typeparam name="TConsumer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddScraperRabbitMqClient(
            this IServiceCollection services,
            Type newPostConsumer = null,
            RabbitMqConfig config = null,
            params Type[] interfacesToIncludeTypeNames)
        {
            return services
                .AddMassTransit(
                    x =>
                    {
                        if (newPostConsumer != null)
                        {
                            x.AddConsumer(newPostConsumer);
                        }
                        
                        x.AddConsumer<ScrapedPostConsumer>();
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            config ??= new RabbitMqConfig();
                            
                            cfg.Host(config.ConnectionString);

                            Type[] interfaces = interfacesToIncludeTypeNames.Concat(new []{ typeof(IMediaItem) }).ToArray();
                            cfg.ConfigureInterfaceJsonSerialization(interfaces);
                            
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