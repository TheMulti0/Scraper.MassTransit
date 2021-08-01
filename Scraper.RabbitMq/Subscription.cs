using System;
using System.Collections.Generic;

namespace Scraper.RabbitMq
{
    public record Subscription : IEqualityComparer<Subscription>
    {
        public string Platform { get; init; }

        public string Id { get; init; }

        public TimeSpan PollInterval { get; init; }

        public bool Equals(Subscription x, Subscription y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }
            return x.Platform == y.Platform && x.Id == y.Id;
        }

        public int GetHashCode(Subscription obj)
        {
            return HashCode.Combine(obj.Platform, obj.Id);
        }
    }
}