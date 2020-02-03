using System;
using Common.Core;
using NodaTime;
using static Common.Events.Create;

namespace Common.EventStore.Lib
{
    public class EventWrapper : IEventWrapper
    {
        internal static EventWrapper Clone(IEventWrapper other)
        {
            return new EventWrapper(other.Content)
            {
                AggregateId = other.Metadata.AggregateId,
                Id = other.Metadata.Id,
                Timestamp = other.Metadata.Timestamp,
                Version = other.Metadata.Version
            };
        }

        internal EventWrapper(IEvent payload)
        {
            Content = payload;
        }

        public EventWrapper(long id, Instant timestamp, Guid aggregateId, int version, IEvent content)
        {
            Id = id;
            Timestamp = timestamp;
            AggregateId = aggregateId;
            Version = version;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public IEventWrapperMetadata Metadata { get => Metadata(Id, AggregateId, Version, Timestamp); }
        internal long Id { get; set; }
        internal Instant Timestamp { get; set; } = SystemClock.Instance.GetCurrentInstant();
        internal Guid AggregateId { get; set; }
        internal int Version { get; set; }
        public IEvent Content { get; }
    }
}
