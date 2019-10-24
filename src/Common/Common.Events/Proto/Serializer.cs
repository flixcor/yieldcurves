using System;
using System.IO;
using System.Linq;
using Common.Core;
using ProtoBuf.Meta;

namespace Common.Events.Proto
{
    public static class Serializer
    {
        static Serializer()
        {
            Setup();
        }

        public static IEvent Deserialize(byte[] byteArray, Type type)
        {
            using var stream = new MemoryStream(byteArray);
            stream.Seek(0, SeekOrigin.Begin);
            var obj = ProtoBuf.Serializer.Deserialize(type, stream);
            return obj is IEvent @event
                ? @event
                : default;
        }

        public static byte[] Serialize(IEvent @event)
        {
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, @event);
            return stream.ToArray();
        }

        private static void Setup()
        {
            var eventTypes = typeof(Serializer).Assembly.GetTypes().Where(x => typeof(IEvent).IsAssignableFrom(x));
            
            foreach (var eventType in eventTypes)
            {
                var @event = RuntimeTypeModel.Default.Add(eventType, false);
                var properties = eventType.GetProperties().Select(p => p.Name).OrderBy(name => name);
                @event.Add(properties.ToArray());
                @event.UseConstructor = false;
            }
        }
    }
}
