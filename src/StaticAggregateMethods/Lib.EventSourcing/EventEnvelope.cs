using System;

namespace Lib.EventSourcing
{
    public record EventEnvelope
    {
        

        public static EventEnvelope Create(string aggregateId, int version, object content)
        {
            var type = typeof(EventEnvelope<>).MakeGenericType(content.GetType());
            var envelope = Activator.CreateInstance(type, new object[] { content }) as EventEnvelope;
            return envelope with { AggregateId = aggregateId, Version = version, Content = content };
        }

        public long Id { get; internal set; }
        public string? AggregateId { get; init; }
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public int Version { get; init; }
        public object? Content { get; init; }
    }

    public record EventEnvelope<T>: EventEnvelope where T : class
    {
        public EventEnvelope(T content)
        {
            Content = content;
        }

        public new T? Content { get; set; }
    }
}
