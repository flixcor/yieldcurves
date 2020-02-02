using System;
using Common.Core;
using NodaTime;

namespace Common.Infrastructure.EventStore
{
    public class EventWrapper : IEventWrapper
    {
        public EventWrapper(IEvent content)
        {
            Content = content;
        }

        public Guid AggregateId { get; set; }

        public IEvent Content { get; }

        public long Id { get; set; }

        public Instant Timestamp { get; set; }

        public int Version { get; set; }

        IEvent IEventWrapper.Content => Content;
    }
}
