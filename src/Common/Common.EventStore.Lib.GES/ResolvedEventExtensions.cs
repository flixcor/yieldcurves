using System;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib.GES.Proto;
using EventStore.Client;
using Google.Protobuf;
using static Common.Events.Create;

namespace Common.EventStore.Lib.GES
{
    internal static class ResolvedEventExtensions
    {
        internal static EventData ToEventData(this IEventWrapper wrapper)
        {
            var typeName = wrapper.Content.GetType().Name;
            var data = Serializer.Serialize(wrapper.Content);
            var metadata = wrapper.Metadata.ToByteArray();

            return new EventData(Uuid.NewUuid(), typeName, data, metadata, false);
        }

        internal static IEventWrapper Deserialize(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;
            var eventName = resolvedEvent.OriginalEvent.EventType;

            var eventHeaders = Serializer.Deserialize<IEventWrapperMetadata>(metadata);

            if (eventHeaders == null)
            {
                return default;
            }

            var typeString = typeof(Events.Create).Namespace + '.' + eventName;
            var type = typeof(Events.Create).Assembly.GetType(typeString);

            var content = Serializer.DeserializeEvent(data, type);

            if (content == null)
            {
                return default;
            }

            var id = resolvedEvent.OriginalEvent.Position.ToInt64().commitPosition;

            return new EventWrapper(id, eventHeaders.Timestamp, eventHeaders.AggregateId, eventHeaders.Version, content);
        }
    }
}
