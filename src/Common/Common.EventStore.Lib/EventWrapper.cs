using System;
using Common.Core;
using NodaTime;
using static Common.Events.Create;

namespace Common.EventStore.Lib
{
    public class EventWrapper : IEventWrapper
    {
        internal EventWrapper(IEvent payload)
        {
            Content = payload;
        }

        public EventWrapper(IEventMetadata metadata, IEvent content)
        {
            Id = metadata.Id;
            Timestamp = metadata.Timestamp;
            AggregateId = metadata.AggregateId;
            Version = metadata.Version;
            Content = content;
        }

        public IEventMetadata Metadata { get => Metadata(Id, AggregateId, Version, Timestamp); }
        internal long Id { get; set; }
        internal Instant Timestamp { get; set; } = SystemClock.Instance.GetCurrentInstant();
        internal Guid AggregateId { get; set; }
        internal int Version { get; set; }
        public IEvent Content { get; }
    }
}
