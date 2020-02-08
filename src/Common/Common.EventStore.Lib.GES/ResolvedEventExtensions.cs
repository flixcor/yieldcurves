using System.Linq;
using Common.Core;
using EventStore.Client;
using static Common.Events.Helpers;

namespace Common.EventStore.Lib.GES
{
    internal static class ResolvedEventExtensions
    {
        internal static EventData ToEventData(this (IEventWrapper, IMetadata) wrapperWithMeta)
        {
            var (wrapper, meta) = wrapperWithMeta;

            var typeName = wrapper.GetContent().GetType().Name;
            var data = Serializer.Serialize(wrapper.GetContent());
            var metadata = Serializer.Serialize(meta);

            return new EventData(Uuid.NewUuid(), typeName, data, metadata, false);
        }

        internal static (IEventWrapper wrapper, IMetadata metadata)? Deserialize(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;
            var eventName = resolvedEvent.OriginalEvent.EventType;

            if (eventTypes.Length != 0 && !eventTypes.Contains(eventName))
            {
                return default;
            }

            var eventHeaders = DeserializeMetadata(metadata);
            var wrapper = DeserializeEventWrapper(data);

            var id = resolvedEvent.OriginalEvent.Position.ToInt64().commitPosition;
            var newWrapper = Wrap(wrapper.AggregateId, wrapper.Timestamp, wrapper.Version, wrapper.GetContent(), id);

            return (newWrapper, eventHeaders);
        }
    }
}
