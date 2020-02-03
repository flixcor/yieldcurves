using System;
using NodaTime;

namespace Common.Core
{
    public interface IEventWrapperMetadata: Google.Protobuf.IMessage
    {
        Guid AggregateId { get; }
        long Id { get; }
        Instant Timestamp { get; }
        int Version { get; }
    }

    public interface IEventWrapper
    {
        IEventWrapperMetadata Metadata { get; }
        IEvent Content { get; }
    }

    public interface IEventWrapper<T> where T : IEvent
    {
        IEventWrapperMetadata Metadata { get; }
        T Content { get; }
    }
}
