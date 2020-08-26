using System;

namespace Lib.EventSourcing
{
    public record EventEnvelope
    {
        public long Id { get; internal set; }
        public string? AggregateId { get; init; }
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public int Version { get; init; }
        public object? Content { get; init; }
    }
}
