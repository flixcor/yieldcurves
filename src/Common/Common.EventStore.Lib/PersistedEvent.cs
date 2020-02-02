using System;
using Common.Core;
using Google.Protobuf;
using NodaTime;

namespace Common.EventStore.Lib
{
    public class PersistedEvent
    {
        private static readonly JsonFormatter s_jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));

        internal static PersistedEvent FromEventWrapper(IEventWrapper eventWrapper)
        {
            return new PersistedEvent
            {
                AggregateId = eventWrapper.AggregateId,
                EventType = eventWrapper.Content.GetType().Name,
                Payload = s_jsonFormatter.Format(eventWrapper.Content) ?? throw new Exception()
            };
        }

        internal EventWrapper ToWrapper()
        {
            var content = GetContent();

            if (content == null)
            {
                throw new Exception();
            }

            return new EventWrapper(content)
            {
                Id = Id,
                AggregateId = AggregateId,
                Timestamp = Timestamp,
                Version = Version
            };
        }

        private IEvent? GetContent()
        {
            var typeString = typeof(Events.Create).Namespace + '.' + EventType;
            var type = typeof(Events.Create).Assembly.GetType(typeString);
            var parser = (MessageParser?)type?.GetProperty("Parser")?.GetValue(null);

            return (IEvent?)parser?.ParseJson(Payload);
        }

        public long Id { get; set; }
        public Instant Timestamp { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
