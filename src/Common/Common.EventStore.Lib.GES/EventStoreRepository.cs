using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    internal class EventStoreRepository : IEventRepository
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventStoreRepository(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public async IAsyncEnumerable<IEventWrapper> GetEvents(Guid aggregateId, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var enumerable = _eventStoreClient.ReadStreamAsync(Direction.Forwards, aggregateId.ToString(),
                                                               StreamRevision.Start, ulong.MaxValue,
                                                               cancellationToken: cancellationToken);

            await foreach (var item in enumerable.WithCancellation(cancellationToken))
            {
                var wrapper = item.Deserialize();
                if (wrapper != null)
                {
                    yield return wrapper;
                }
            }
        }

        public async Task SaveEvents(CancellationToken cancellationToken, params IEventWrapper[] events)
        {
            var streamName = events.First().AggregateId.ToString();

            var expectedVersion = events.Min(x => x.Version) - 1;
            var revision = new StreamRevision((ulong)expectedVersion);

            var eventData = events.Select(x => x.ToEventData());

            var result = await _eventStoreClient.ConditionalAppendToStreamAsync(streamName, revision, eventData, cancellationToken: cancellationToken);

            if (result.Status == ConditionalWriteStatus.VersionMismatch)
            {
                throw new Exception();
            }
        }
    }
}
