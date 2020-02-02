using System;
using NodaTime;

namespace Common.Core
{
    public interface IEventWrapperMetadata
    {
        Guid AggregateId { get; }
        long Id { get; }
        Instant Timestamp { get; }
        int Version { get; }
    }

    public interface IEventWrapper: IEventWrapperMetadata
    {
        IEvent Content { get; }
    }

    public interface IEventWrapper<T> : IEventWrapperMetadata where T : IEvent
    {
        T Content { get; }
    }
}
