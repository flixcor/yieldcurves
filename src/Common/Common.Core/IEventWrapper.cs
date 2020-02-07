using System;
using NodaTime;

namespace Common.Core
{
    public interface IEventWrapper
    {
        long Id { get; }
        Guid AggregateId { get; }
        Instant Timestamp { get; }
        int Version { get; }
        IEvent GetContent();
    }
}
