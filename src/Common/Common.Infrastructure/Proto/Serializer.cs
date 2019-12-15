using System;
using System.IO;
using System.Linq;
using Common.Core;
using Common.Events;
using Google.Protobuf;
using ProtoBuf.Meta;

namespace Common.Infrastructure.Proto
{
    public static class Serializer
    {
        static Serializer()
        {
            Setup();
        }

        public static IEvent DeserializeEvent(byte[] byteArray, Type type)
        {
            if (!typeof(IEvent).IsAssignableFrom(type) || !typeof(Google.Protobuf.IMessage).IsAssignableFrom(type))
            {
                return default;
            }

            using var stream = new MemoryStream(byteArray);

            var message = (Google.Protobuf.IMessage)Activator.CreateInstance(type);
            message.MergeFrom(stream);
            var @event = (IEvent)message;

            return @event ?? default;
        }

        public static T Deserialize<T>(byte[] byteArray)
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
            eventHeaders.AddField(1, nameof(EventHeaders.CommitId));
            eventHeaders.AddField(2, nameof(EventHeaders.AggregateClrTypeName));
            eventHeaders.AddField(3, nameof(EventHeaders.EventClrTypeName));
        }
    }
}
