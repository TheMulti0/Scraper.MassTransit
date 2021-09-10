using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Net;

namespace Scraper.MassTransit.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollectionBusConfigurator AddScraperMassTransitClient(
            this IServiceCollectionBusConfigurator configurator)
        {
            configurator.AddConsumer<ScrapedPostConsumer>();

            configurator.Collection
                .AddSingleton<ScrapedPostsService>()
                .AddSingleton<IScraperService, ScraperMassTransitClient>();

            return configurator;
        }
    }
}