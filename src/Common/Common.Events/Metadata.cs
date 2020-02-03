using System;
using Common.Core;
using Google.Protobuf.WellKnownTypes;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace Common.Events
{
    internal partial class Metadata : IEventMetadata, IMessage
    {
        public Metadata(long id, Instant timestamp, Guid aggregateId, int version)
        {
            Id = id;
            Timestamp = timestamp.ToTimestamp();
            AggregateId = aggregateId;
            Version = version;
        }

        Guid IEventMetadata.AggregateId => AggregateId;

        Instant IEventMetadata.Timestamp => Timestamp.ToInstant();
    }
}
