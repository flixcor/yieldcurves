using System;
using System.Text;
using Common.Core;
using Common.Infrastructure.Proto;
using EventStore.ClientAPI;
using Newtonsoft.Json.Linq;

namespace Common.Infrastructure.Extensions
{
    internal static class ResolvedEventExtensions
    {
        private const string EventClrTypeHeader = "EventClrTypeName";

        internal static IEvent Deserialize(this ResolvedEvent resolvedEvent)
        {
            var metadata = resolvedEvent.OriginalEvent.Metadata;
            var data = resolvedEvent.OriginalEvent.Data;

            var metadataString = Encoding.UTF8.GetString(metadata);

            if (!string.IsNullOrWhiteSpace(metadataString))
            {
                var eventClrTypeName = JObject.Parse(metadataString).Property(EventClrTypeHeader)?.Value;

                if (eventClrTypeName != null)
                {
                    return Serializer.Deserialize(data, Type.GetType((string)eventClrTypeName));
                }
            }

            return default;
        }
    }
}
