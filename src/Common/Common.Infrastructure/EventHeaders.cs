using System;
using NodaTime;

namespace Common.Infrastructure
{
    public class EventHeaders
    {
        public int Version { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; }
        public Instant Timestamp { get; set; }
    }
}
