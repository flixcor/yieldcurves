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
    public class InMemoryEventStore : IEventStore
    {
        private class InMemoryStream
        {
            public InMemoryStream(IEnumerable<EventWrapper> events)
            {
                Events = events.ToImmutableArray();
            }

            public ImmutableArray<EventWrapper> Events { get; }
        }

        private readonly Channel<EventWrapper> _channel = Channel.CreateBounded<EventWrapper>(1000);

        private readonly ConcurrentDictionary<string, InMemoryStream> _streams = new ConcurrentDictionary<string, InMemoryStream>();

        public IAsyncEnumerable<EventWrapper> Subscribe(CancellationToken token) => _channel.Reader.ReadAllAsync(token);

        public async Task Save(string stream, CancellationToken cancellationToken = default, params EventWrapper[] events)
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
                    await _channel.Writer.WriteAsync(item, cancellationToken);
                }
            }
        }

        public async IAsyncEnumerable<EventWrapper> Get(string stream, CancellationToken cancellation = default)
        {
            if (_streams.TryGetValue(stream, out var r))
            {
                foreach (var item in r.Events)
                {
                    yield return item;
                }
            }
        }
    }
}
