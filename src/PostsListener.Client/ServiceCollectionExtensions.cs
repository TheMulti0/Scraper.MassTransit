using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.MassTransit.Common;

namespace PostsListener.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollectionBusConfigurator AddPostsListenerClient(
            this IServiceCollectionBusConfigurator configurator)
        {
            return configurator.AddPostsListenerClient(null);
        }
        
        public static IServiceCollectionBusConfigurator AddPostsListenerClient<TNewPostConsumer>(
            this IServiceCollectionBusConfigurator configurator)
            where TNewPostConsumer : IConsumer<NewPost>
        {
            return configurator.AddPostsListenerClient(typeof(TNewPostConsumer));
        }

        private static IServiceCollectionBusConfigurator AddPostsListenerClient(
            this IServiceCollectionBusConfigurator configurator,
            Type newPostConsumer)
        {
            if (newPostConsumer != null)
            {
                configurator.AddConsumer(newPostConsumer);
            }

            configurator.Collection
                .AddSingleton<INewPostSubscriptionsClient, NewPostSubscriptionsClient>();

            return configurator;
        }
    }
}