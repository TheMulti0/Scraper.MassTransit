using System;
using System.Collections.Generic;
using System.Threading;
using Scraper.Net;
using Scraper.Net.Stream;

namespace PostsListener
{
    internal class PostStream : IPostStream
    {
        private readonly IObservable<Post> _stream;
        private readonly IPostStream _postStream;

        public PostStream(IObservable<Post> stream, IPostStream postStream)
        {
            _stream = stream;
            _postStream = postStream;
        }

        public IDisposable Subscribe(IObserver<Post> observer) => _stream.Subscribe(observer);

        public IAsyncEnumerable<Post> UpdateAsync(CancellationToken ct) => _postStream.UpdateAsync(ct);
    }
}