using System;
using System.IO;
using Common.Core;
using Google.Protobuf;
using ProtoBuf.Meta;

namespace Common.EventStore.Lib.GES.Proto
{
    public static class Serializer
    {
        public static IEvent DeserializeEvent(byte[] byteArray, Type type)
        {
            var parser = (MessageParser)type?.GetProperty("Parser")?.GetValue(null);
            var @event = (IEvent)parser?.ParseFrom(byteArray);

            return @event;
        }

        public static T Deserialize<T>(byte[] byteArray) where T : Google.Protobuf.IMessage
        {
            var parser = (MessageParser)typeof(T).GetProperty("Parser")?.GetValue(null);
            return (T)parser.ParseFrom(byteArray);
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
    }
}
