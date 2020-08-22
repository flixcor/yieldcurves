using NodaTime;

namespace ExampleService.Shared
{
    public record EventWrapper
    {
        public long Id { get; internal set; }
        public string? AggregateId { get; init; }
        public Instant Timestamp { get; init; }
        public int Version { get; init; }
        public object? Content { get; init; }
    }
}
