using System;
using NodaTime;

namespace ExampleService.Shared
{
    public record CommandEnvelope<T> where T : class
    {
        public string CausationId { get; init; } = Guid.NewGuid().ToString();
        public string? CorrelationId { get; init; }
        public Instant TimeStamp { get; init; }
        public string? AggregateId { get; init; } = Guid.NewGuid().ToString();
        public T? Command { get; init; }
    }
}
