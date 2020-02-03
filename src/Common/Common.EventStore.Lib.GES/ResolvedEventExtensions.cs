using System.Linq;
using Common.Core;
using Common.EventStore.Lib.Proto;
using EventStore.Client;
using Google.Protobuf;

namespace Common.EventStore.Lib.GES
{
    internal static class ResolvedEventExtensions
    {
        internal static EventData ToEventData(this IEventWrapper wrapper)
        {
            var typeName = wrapper.Content.GetType().Name;
            var data = wrapper.Content.ToByteArray();
            var metadata = wrapper.Metadata.ToByteArray();

            return new EventData(Uuid.NewUuid(), typeName, data, metadata, false);
        }

        internal static IEventWrapper? Deserialize(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;
            var eventName = resolvedEvent.OriginalEvent.EventType;

            if (eventTypes.Length != 0 && !eventTypes.Contains(eventName))
            {
                return default;
            }

            var eventHeaders = Serializer.Deserialize<IEventMetadata>(metadata);

            var content = Serializer.DeserializeEvent(data, eventName);

            if (content == null)
            {
                return default;
            }

            var id = resolvedEvent.OriginalEvent.Position.ToInt64().commitPosition;

            return new EventWrapper(Events.Create.Metadata(id, eventHeaders.AggregateId, eventHeaders.Version, eventHeaders.Timestamp), content);
        }
    }
}
