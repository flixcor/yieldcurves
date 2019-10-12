using Common.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Infrastructure.Extensions
{
    internal static class ResolvedEventExtensions
    {
        private const string EventClrTypeHeader = "EventClrTypeName";

        internal static Event Deserialize(this ResolvedEvent resolvedEvent)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var metadataString = Encoding.UTF8.GetString(metadata);

            if (!string.IsNullOrWhiteSpace(metadataString))
            {
                var eventClrTypeName = JObject.Parse(metadataString).Property(EventClrTypeHeader)?.Value;

                if (eventClrTypeName != null)
                {
                    var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
                    if ((@event is Event))
                    {
                        return @event as Event;
                    }
                }
            }

            return default;
        }
    }
}
