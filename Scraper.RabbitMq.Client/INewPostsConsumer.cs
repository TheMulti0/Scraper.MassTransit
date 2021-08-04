using System;

namespace Scraper.RabbitMq.Client
{
    public interface INewPostsConsumer
    {
        IObservable<NewPost> NewPosts { get; }
    }
}