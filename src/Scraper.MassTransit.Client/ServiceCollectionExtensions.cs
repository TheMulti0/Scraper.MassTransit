using System;
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
                .AddSingleton<ScrapedPostsService>()
                .AddSingleton<IScraperService>(
                    provider => ActivatorUtilities.CreateInstance<ScraperMassTransitClient>(provider, getPostsTimeout));

            return configurator;
        }
    }
}