using System;
using Common.Core;
using NodaTime;

namespace Common.EventStore.Lib
{
    internal class EventWrapper : IEventWrapper
    {
        internal static EventWrapper Clone(IEventWrapper other)
        {
            return new EventWrapper(other.Content)
            {
                AggregateId = other.AggregateId,
                Id = other.Id,
                Timestamp = other.Timestamp,
                Version = other.Version
            };
        }

        internal EventWrapper(IEvent payload)
        {
            Content = payload;
        }

        public long Id { get; internal set; }
        public Instant Timestamp { get; internal set; } = SystemClock.Instance.GetCurrentInstant();
        public Guid AggregateId { get; internal set; }
        public int Version { get; internal set; }
        public IEvent Content { get; }
    }
}
