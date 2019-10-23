using System;

namespace Common.Core
{
    public interface IEvent : IMessage
    {
        Guid AggregateId { get; }
        int Version { get; }
    }
}
