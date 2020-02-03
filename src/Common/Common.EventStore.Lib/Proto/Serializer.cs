using System;
using System.Text.Json;
using Common.Core;
using Google.Protobuf;

namespace Common.EventStore.Lib.Proto
{
    public static class Serializer
    {
        public static IEvent? DeserializeEvent(byte[] byteArray, string eventName)
        {
            var typeString = typeof(Events.Create).Namespace + '.' + eventName;
            var type = typeof(Events.Create).Assembly.GetType(typeString);

            if (type == null)
            {
                return default;
            }

            var parser = (MessageParser?)type?.GetProperty("Parser")?.GetValue(null);
            var @event = (IEvent?)parser?.ParseFrom(byteArray);

            return @event;
        }

        public static T Deserialize<T>(byte[] byteArray) where T : class, Google.Protobuf.IMessage
        {
            var parser = (MessageParser?)typeof(T).GetProperty("Parser")?.GetValue(null);
            return (T?)parser?.ParseFrom(byteArray) ?? throw new Exception();
        }

        public static object Deserialize(byte[] byteArray, Type type)
        {
            var parser = (MessageParser?)type.GetProperty("Parser")?.GetValue(null);

            return parser != null
                ? parser.ParseFrom(byteArray)
                : JsonSerializer.Deserialize(byteArray, type);
        }

        public static byte[] Serialize(object obj)
        {
            return obj is Google.Protobuf.IMessage proto
                ? proto.ToByteArray()
                : JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType());
        }
    }
}
