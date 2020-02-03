using System;
using System.IO;
using Common.Core;
using Google.Protobuf;
using ProtoBuf.Meta;

namespace Common.EventStore.Lib.GES.Proto
{
    public static class Serializer
    {
        static Serializer()
        {
            Setup();
        }

        public static IEvent DeserializeEvent(byte[] byteArray, Type type)
        {
            var parser = (MessageParser)type?.GetProperty("Parser")?.GetValue(null);
            var @event = (IEvent)parser?.ParseFrom(byteArray);

            return @event;
        }

        public static T Deserialize<T>(byte[] byteArray) where T : class
        {
            using var stream = new MemoryStream(byteArray);
            stream.Seek(0, SeekOrigin.Begin);
            var obj = ProtoBuf.Serializer.Deserialize(typeof(T), stream);
            return obj is T t
                ? t
                : default;
        }

        public static byte[] Serialize(object obj)
        {
            if (obj is Google.Protobuf.IMessage proto)
            {
                return proto.ToByteArray();
            }

            using var stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            ProtoBuf.Serializer.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static void Setup()
        {
            var eventHeaders = RuntimeTypeModel.Default.Add(typeof(EventHeaders), false);
            eventHeaders.AddField(1, nameof(EventHeaders.AggregateId));
            eventHeaders.AddField(2, nameof(EventHeaders.EventType));
            eventHeaders.AddField(3, nameof(EventHeaders.Timestamp));
            eventHeaders.AddField(4, nameof(EventHeaders.Version));
        }
    }
}
