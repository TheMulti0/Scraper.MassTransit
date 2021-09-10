using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollectionBusConfigurator AddScraperMassTransitClient(
            this IServiceCollectionBusConfigurator configurator,
            TimeSpan? getPostsTimeout = null)
        {
            configurator.AddConsumer<ScrapedPostConsumer>();

            configurator.Collection
                .AddSingleton<ScrapedPostsManager>()
                .AddSingleton<IScraperService>(
                    provider => new ScraperMassTransitClient(
                        provider.GetRequiredService<IBus>(),
                        provider.GetRequiredService<ScrapedPostsManager>(),
                        getPostsTimeout));

            return configurator;
        }
    }
}