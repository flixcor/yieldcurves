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

        public async Task Save(CancellationToken cancellationToken, params IEventWrapper[] events)
        {
            var streamName = events.First().Metadata.AggregateId.ToString();

            var expectedVersion = events.Min(x => x.Metadata.Version) - 1;
            var revision = new StreamRevision((ulong)expectedVersion);

            var eventData = events.Select(x => x.ToEventData());

            var result = await _eventStoreClient.ConditionalAppendToStreamAsync(streamName, revision, eventData, cancellationToken: cancellationToken);

            if (result.Status == ConditionalWriteStatus.VersionMismatch)
            {
                throw new Exception();
            }
        }

        public IAsyncEnumerable<IEventWrapper> Get(CancellationToken cancellation) => Get(EventFilter.None, cancellation);

        public IAsyncEnumerable<IEventWrapper> Get(IEventFilter eventFilter, CancellationToken cancellation)
        {
            var checkpoint = eventFilter.Checkpoint;

            var resolvedEvents = eventFilter.AggregateId.HasValue
                ? GetStream(eventFilter.AggregateId.Value, eventFilter.Checkpoint, eventFilter.EventTypes, cancellation)
                : GetAll(eventFilter, checkpoint, cancellation);

            return ToEventWrapper(resolvedEvents, cancellation);
        }

        private IAsyncEnumerable<ResolvedEvent> GetAll(IEventFilter eventFilter, long? checkpoint, CancellationToken cancellation)
        {
            IAsyncEnumerable<ResolvedEvent> resolvedEvents;
            var filter = eventFilter.ToGESFilter();

            var allPosition = checkpoint.HasValue
                ? new Position((ulong)checkpoint, (ulong)checkpoint)
                : Position.Start;

            resolvedEvents = _eventStoreClient.ReadAllAsync(Direction.Forwards, allPosition, ulong.MaxValue,
                filter: filter, cancellationToken: cancellation);
            return resolvedEvents;
        }

        private IAsyncEnumerable<ResolvedEvent> GetStream(Guid id, long? revision, IEnumerable<string> eventTypes, CancellationToken cancellation)
        {
            var streamPosition = revision.HasValue
                    ? StreamRevision.FromInt64(revision.Value)
                    : StreamRevision.Start;

            var resolvedEvents = _eventStoreClient.ReadStreamAsync(Direction.Forwards,
                    id.ToString(), streamPosition, ulong.MaxValue, cancellationToken: cancellation);

            if (eventTypes.Any())
            {
                resolvedEvents = resolvedEvents.Where(x => eventTypes.Contains(x.OriginalEvent.EventType));
            }

            return resolvedEvents;
        }

        private async IAsyncEnumerable<IEventWrapper> ToEventWrapper(IAsyncEnumerable<ResolvedEvent> input,
                                                                     [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await foreach (var item in input.WithCancellation(cancellationToken))
            {
                var wrapper = item.Deserialize();
                if (wrapper != null)
                {
                    yield return wrapper;
                }
            }
        }
    }
}
