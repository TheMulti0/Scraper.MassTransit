using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
        private readonly ConcurrentDictionary<Subscription, IDisposable> _subscriptions;

        public SubscriptionsManager(
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

        public void Add(Subscription subscription)
        {
            if (_subscriptions.ContainsKey(subscription))
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            IObservable<Post> stream = _streamer
                .Stream(subscription.Id, subscription.Platform, subscription.PollInterval);

            void PublishPost(Post post) => _bus.Publish(new PostReceived
            {
                Post = post,
                Platform = subscription.Platform
            });

            IDisposable disposable = stream.Subscribe(PublishPost);

            if (!_subscriptions.TryAdd(subscription, disposable))
            {
                throw new InvalidOperationException("Failed to add subscription");
            }
        }

        public void Remove(Subscription subscription)
        {
            if (!_subscriptions.TryRemove(subscription, out IDisposable disposable))
            {
                throw new InvalidOperationException("Failed to remove subscription");
            }
            
            disposable?.Dispose();
        }
    }
}