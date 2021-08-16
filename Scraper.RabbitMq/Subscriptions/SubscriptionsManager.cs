using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions;
using MassTransit;
using Scraper.Net;
using Scraper.Net.Stream;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq
{
    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly PostsStreamer _streamer;
        private readonly IBus _bus;
        private readonly ISubscriptionsPersistence _subscriptionsPersistence;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;

        public SubscriptionsManager(
            PostsStreamer streamer,
            IBus bus,
            ISubscriptionsPersistence subscriptionsPersistence)
        {
            _streamer = streamer;
            _bus = bus;
            _subscriptionsPersistence = subscriptionsPersistence;
            _subscriptions = new ConcurrentDictionary<Subscription, IDisposable>();
        }

        public IDictionary<Subscription, IDisposable> Get()
        {
            return _subscriptions;
        }

        public void AddOrUpdate(Subscription subscription)
        {
            _subscriptions.AddOrUpdate(
                subscription,
                StreamSubscription,
                (s, _) => StreamSubscription(s));
            
            _subscriptionsPersistence.AddOrUpdate(subscription);
        }

        private IDisposable StreamSubscription(Subscription subscription)
        {
            IObservable<Post> stream = _streamer
                .Stream(subscription.Id, subscription.Platform, subscription.PollInterval);

            async Task PublishPost(Post post)
            {
                await _bus.Publish(
                    new NewPost
                    {
                        Post = post,
                        Platform = subscription.Platform
                    });
            }

            return stream.SubscribeAsync(PublishPost);
        }

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.TryRemove(subscription, out IDisposable disposable))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }

            _subscriptionsPersistence.Remove(subscription);
            
            disposable?.Dispose();
        }
    }
}