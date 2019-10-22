using System;

namespace Common.Core
{
    public interface IEvent : IMessage
    {
        Guid Id { get; }
        int Version { get; }
    }
}
