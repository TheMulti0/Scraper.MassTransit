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
    public class StreamerManager : ISubscriptionsManager
    {
        private readonly PostsStreamer _streamer;
        private readonly IBus _bus;
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;

        public StreamerManager(
            PostsStreamer streamer,
            IBus bus)
        {
            _streamer = streamer;
            _bus = bus;
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
                (s, old) =>
                {
                    old.Dispose();
                    return StreamSubscription(s);
                });
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
            if (!_subscriptions.ContainsKey(subscription))
            {
                throw new KeyNotFoundException();
            }
        
            if (!_subscriptions.TryRemove(subscription, out IDisposable disposable))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            disposable?.Dispose();
        }
    }
}