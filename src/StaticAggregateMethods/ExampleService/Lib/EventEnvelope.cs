using NodaTime;

namespace ExampleService.Shared
{
    public record EventEnvelope
    {
        public long Id { get; internal set; }
        public string? AggregateId { get; init; }
        public Instant Timestamp { get; init; } = SystemClock.Instance.GetCurrentInstant();
        public int Version { get; init; }
        public object? Content { get; init; }
    }
}
