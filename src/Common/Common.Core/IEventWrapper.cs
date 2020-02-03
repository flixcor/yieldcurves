using System;
using NodaTime;

namespace Common.Core
{
    public interface IEventMetadata: Google.Protobuf.IMessage
    {
        Guid AggregateId { get; }
        long Id { get; }
        Instant Timestamp { get; }
        int Version { get; }
    }

    public interface IEventWrapper
    {
        IEventMetadata Metadata { get; }
        IEvent Content { get; }
    }

    public interface IEventWrapper<T> where T : IEvent
    {
        IEventMetadata Metadata { get; }
        T Content { get; }
    }
}
