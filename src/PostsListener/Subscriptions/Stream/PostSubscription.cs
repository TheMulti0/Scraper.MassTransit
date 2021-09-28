using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace PostsListener
{
    public class PostSubscription
    {
        private readonly Subject<Unit> _subject;
        private readonly IDisposable _subscription;

        public PostSubscription(Subject<Unit> subject, IDisposable subscription)
        {
            _subject = subject;
            _subscription = subscription;
        }

        public void TriggerPoll()
        {
            _subject.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}