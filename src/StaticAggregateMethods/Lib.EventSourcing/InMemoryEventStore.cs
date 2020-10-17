using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lib.EventSourcing
{
    public class InMemoryEventStore : IEventStore, IDisposable
    {
        private class InMemoryEventStream
        {
            public InMemoryEventStream(IEnumerable<EventEnvelope> events)
            {
                Events = events.ToImmutableArray();
            }

            public ImmutableArray<EventEnvelope> Events { get; }
        }

        private readonly Channel<EventEnvelope> _channel = Channel.CreateBounded<EventEnvelope>(1000);

        private readonly ConcurrentDictionary<string, InMemoryEventStream> _streams = new ConcurrentDictionary<string, InMemoryEventStream>();

        private long _position = -1;

        public IAsyncEnumerable<EventEnvelope> Subscribe(CancellationToken token) => _channel.Reader.ReadAllAsync(token);

        private EventEnvelope IncrementId(EventEnvelope envelope) => envelope with { Position = Interlocked.Increment(ref _position) };

        public async Task<long?> Save(string stream, CancellationToken cancellationToken = default, params EventEnvelope[] events)
        {
            if (events.Any())
            {
                List<EventEnvelope>? addedEvents = null;

                _streams.AddOrUpdate(
                    key: stream,
                    addValueFactory: _ =>
                    {
                        addedEvents = events.Select(IncrementId).ToList();
                        return new InMemoryEventStream(addedEvents);
                    },
                    updateValueFactory: (_, value) =>
                    {
                        var first = events.First();
                        if (value.Events.Length != first.Version)
                        {
                            throw new Exception();
                        }

                        addedEvents = events.Select(IncrementId).ToList();
                        return new InMemoryEventStream(value.Events.Concat(addedEvents));
                    });

                if (addedEvents == null)
                {
                    return null;
                }

                foreach (var item in addedEvents)
                {
                    await _channel.Writer.WriteAsync(item, cancellationToken);
                }

                return addedEvents.Max(x => x.Position);
            }

            return null;
        }

        public IAsyncEnumerable<EventEnvelope> Get(string stream, CancellationToken cancellation = default) => 
            new FakeAsyncEnumerable<EventEnvelope>(GetSync(stream));

        private IEnumerable<EventEnvelope> GetSync(string stream) =>
            _streams.TryGetValue(stream, out var r) 
                ? r.Events 
                : Enumerable.Empty<EventEnvelope>();

        public void Dispose()
        {
            _channel.Writer.Complete();
            GC.SuppressFinalize(this);
        }

        private class FakeAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> _enumerable;

            public FakeAsyncEnumerable(IEnumerable<T> enumerable) => _enumerable = enumerable;

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) 
                => new FakeAsyncEnumerator<T>(_enumerable.GetEnumerator());
        }

        private class FakeAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public FakeAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

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
