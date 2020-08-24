using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public class InMemoryEventStore : IEventStore, IDisposable
    {
        private class InMemoryStream
        {
            public InMemoryStream(IEnumerable<EventEnvelope> events)
            {
                Events = events.ToImmutableArray();
            }

            public ImmutableArray<EventEnvelope> Events { get; }
        }

        private readonly Channel<EventEnvelope> _channel = Channel.CreateBounded<EventEnvelope>(1000);

        private readonly ConcurrentDictionary<string, InMemoryStream> _streams = new ConcurrentDictionary<string, InMemoryStream>();

        private long _position;

        public IAsyncEnumerable<EventEnvelope> Subscribe(CancellationToken token) => _channel.Reader.ReadAllAsync(token);

        public async Task Save(string stream, CancellationToken cancellationToken = default, params EventEnvelope[] events)
        {
            if (events.Any())
            {
                _streams.AddOrUpdate(stream, new InMemoryStream(events), (k, v) =>
                {
                    var first = events.First();
                    if (v.Events.Length != first.Version)
                    {
                        throw new Exception();
                    }
                    return new InMemoryStream(v.Events.Concat(events));
                });
                foreach (var item in events)
                {
                    item.Id = Interlocked.Increment(ref _position);
                    await _channel.Writer.WriteAsync(item, cancellationToken);
                }
            }
        }

        public IAsyncEnumerable<EventEnvelope> Get(string stream, CancellationToken cancellation = default) => 
            new FakeAsyncEnumerable<EventEnvelope>(GetSync(stream));

        private IEnumerable<EventEnvelope> GetSync(string stream) =>
            _streams.TryGetValue(stream, out var r) 
                ? r.Events 
                : Enumerable.Empty<EventEnvelope>();

        public void Dispose() => _channel.Writer.Complete();

        private class FakeAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> _enumerable;

            public FakeAsyncEnumerable(IEnumerable<T> enumerable) => _enumerable = enumerable;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) 
                => new TestDbAsyncEnumerator<T>(_enumerable.GetEnumerator());
        }

        private class TestDbAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestDbAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }

            public T Current => _inner.Current;
        }
    }
}
