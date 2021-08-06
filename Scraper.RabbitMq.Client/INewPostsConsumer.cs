using System;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    public interface INewPostsConsumer
    {
        IObservable<RabbitMqMessage<NewPost>> NewPosts { get; }
    }
}