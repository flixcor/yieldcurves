using System;
using Common.Core;
using Common.EventStore.Lib.Proto;
using NodaTime;

namespace Common.EventStore.Lib.EfCore
{
    public class PersistedEvent
    {

        internal static PersistedEvent FromEventWrapper(IEventWrapper eventWrapper)
        {
            return new PersistedEvent
            {
                AggregateId = eventWrapper.Metadata.AggregateId,
                EventType = eventWrapper.Content.GetType().Name,
                Payload = Serializer.Serialize(eventWrapper.Content)
            };
        }

        internal EventWrapper ToWrapper()
        {
            var content = GetContent();

            if (content == null)
            {
                throw new Exception();
            }

            return new EventWrapper(Events.Create.Metadata(Id, AggregateId, Version, Timestamp), content);
        }

        private IEvent? GetContent()
        {
            return Serializer.DeserializeEvent(Payload, EventType);
        }

        public long Id { get; set; }
        public Instant Timestamp { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; } = string.Empty;
        public byte[] Payload { get; set; } = Array.Empty<byte>();
    }
}
