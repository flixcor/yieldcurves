using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.Infrastructure.Proto;
using EventStore.ClientAPI;

namespace Common.Infrastructure.Extensions
{
    internal static class ResolvedEventExtensions
    {
        internal static IEvent Deserialize(this ResolvedEvent resolvedEvent)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var eventHeaders = Serializer.Deserialize<EventHeaders>(metadata);

            if (eventHeaders != null)
            {
                var eventClrTypeName = eventHeaders.EventClrTypeName;

                if (eventClrTypeName != null)
                {
                    return Serializer.DeserializeEvent(data, Type.GetType(eventClrTypeName));
                }
            }

            return default;
        }

        internal static IEnumerable<IEvent> Deserialize(this IEnumerable<ResolvedEvent> resolvedEvents, params string[] eventTypes)
        {
            foreach (var resolvedEvent in resolvedEvents)
            {
                var metadata = resolvedEvent.OriginalEvent.Metadata;
                var data = resolvedEvent.OriginalEvent.Data;

                var eventHeaders = Serializer.Deserialize<EventHeaders>(metadata);

                if (eventHeaders != null)
                {
                    var eventClrTypeName = eventHeaders.EventClrTypeName;

                    if (eventClrTypeName != null)
                    {
                        var type = Type.GetType(eventClrTypeName);

                        if (eventTypes.Contains(type.Name))
                        {
                            var @event = Serializer.DeserializeEvent(data, type);

                            if (@event != null)
                            {
                                yield return @event;
                            }
                        }
                    }
                }
            }
        }
    }
}
