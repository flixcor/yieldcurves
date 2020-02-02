using System;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib;
using Common.Infrastructure.EventStore;
using Common.Infrastructure.Proto;
using EventStore.Client;

namespace Common.Infrastructure.Extensions
{
    internal static class ResolvedEventExtensions
    {
        internal static EventData ToEventData(this IEventWrapper wrapper)
        {
            var typeName = wrapper.Content.GetType().Name;
            var data = Serializer.Serialize(wrapper.Content);
            var eventHeaders = new EventHeaders
            {
                AggregateId = wrapper.AggregateId,
                Timestamp = wrapper.Timestamp,
                Version = wrapper.Version
            };
            var metadata = Serializer.Serialize(eventHeaders);

            return new EventData(Uuid.NewUuid(), typeName, data, metadata, false);
        }

        internal static IEventWrapper? Deserialize(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var eventHeaders = Serializer.Deserialize<EventHeaders>(metadata);

            if (eventHeaders == null || !eventTypes.Contains(eventHeaders.EventType))
            {
                return default;
            }

            var eventName = eventHeaders.EventType;
            var typeString = typeof(Events.Create).Namespace + '.' + eventName;
            var type = typeof(Events.Create).Assembly.GetType(typeString);

            var content = Serializer.DeserializeEvent(data, type);

            if (content == null)
            {
                return default;
            }

            return new EventWrapper(content)
            {
                AggregateId = eventHeaders.AggregateId,
                Id = resolvedEvent.OriginalEvent.Position.ToInt64().commitPosition,
                Timestamp = eventHeaders.Timestamp,
                Version = eventHeaders.Version
            };
        }
    }
}
