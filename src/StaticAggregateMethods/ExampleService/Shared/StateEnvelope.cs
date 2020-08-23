using System;

namespace ExampleService.Shared
{
    public record StateEnvelope<T> where T : new()
    {
        public T State { get; init; } = new T();
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public int Version { get; init; } = -1;
    }
}
