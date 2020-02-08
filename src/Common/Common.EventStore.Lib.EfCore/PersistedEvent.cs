using System;
using Common.Core;
using NodaTime;
using static Common.Events.Helpers;

namespace Common.EventStore.Lib.EfCore
{
    public class PersistedEvent
    {

        internal static PersistedEvent FromEventWrapper((IEventWrapper, IMetadata) eventWrapper)
        {
            (var wrapper, var metadata) = eventWrapper;

            return new PersistedEvent
            {
                AggregateId = wrapper.AggregateId,
                Version = wrapper.Version,
                Timestamp = wrapper.Timestamp,
                Payload = Serializer.Serialize(wrapper),
                Metadata = Serializer.Serialize(metadata),
                EventType = wrapper.GetContent().GetType().Name
            };
        }

        internal (IEventWrapper, IMetadata) ToWrapper()
        {
            var wrapper = Serializer.DeserializeEvent(Payload);

            var newWrapper = Wrap(AggregateId, Timestamp, Version, wrapper.GetContent(), Id);
            var metadata = Serializer.Deserialize<IMetadata>(Metadata);

            return (newWrapper, metadata);
        }

        public long Id { get; set; }
        public Instant Timestamp { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; } = string.Empty;
        public byte[] Metadata { get; set; } = Array.Empty<byte>();
        public byte[] Payload { get; set; } = Array.Empty<byte>();
    }
}
