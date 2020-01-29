using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Common.Infrastructure.Proto;
using EventStore.Client;

namespace Common.Infrastructure.Extensions
{
    internal static class ResolvedEventExtensions
    {
        internal static (Position?, string, IEvent) Deserialize(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var eventHeaders = Serializer.Deserialize<EventHeaders>(metadata);

            if (eventHeaders != null)
            {
                var eventName = eventHeaders.EventName;
                var eventTypeName = typeof(Events.Create).Namespace + '.' + eventName;

                if (!string.IsNullOrWhiteSpace(eventName) && (!eventTypes.Any() || eventTypes.Contains(eventName)))
                {
                    var type = typeof(Events.Create).Assembly.GetType(eventTypeName);

                    if (type != null)
                    {
                        var @event = Serializer.DeserializeEvent(data, type);
                        return (resolvedEvent.OriginalPosition, eventName, @event);
                    } 
                }
            }

            return default;
        }


        internal static (Position?, string, byte[]) ResolveEventBytes(this ResolvedEvent resolvedEvent, params string[] eventTypes)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var eventHeaders = Serializer.Deserialize<EventHeaders>(metadata);

            if (eventHeaders != null)
            {
                var eventName = eventHeaders.EventName;

                if (!string.IsNullOrWhiteSpace(eventName) && (!eventTypes.Any() || eventTypes.Contains(eventName)))
                {
                    return (resolvedEvent.OriginalPosition, eventName, data);
                }
            }

            return default;
        }

        internal static IEnumerable<(Position?, string, byte[])> ResolveEventBytes(this IEnumerable<ResolvedEvent> resolvedEvents, params string[] eventTypes)
        {
            return resolvedEvents
                .Select(e => e.ResolveEventBytes(eventTypes))
                .Where((e) => e.Item3 != default);
        }

        internal static IEnumerable<(Position?, string, IEvent)> Deserialize(this IEnumerable<ResolvedEvent> resolvedEvents, params string[] eventTypes)
        {
            return resolvedEvents
                .Select(e => e.Deserialize(eventTypes))
                .Where((e) => e.Item3 != default);
        }
    }
}
