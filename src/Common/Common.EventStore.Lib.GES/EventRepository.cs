using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common.Core;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    internal class EventRepository : IEventWriteRepository, IEventReadRepository
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly UserCredentials _userCredentials;

        public EventRepository(EventStoreClient eventStoreClient, UserCredentials userCredentials)
        {
            _eventStoreClient = eventStoreClient;
            _userCredentials = userCredentials;
        }

        public async Task Save(CancellationToken cancellationToken = default, params (IEventWrapper, IMetadata)[] events)
        {
            var streamName = events.First().Item1.AggregateId.ToString();

            var expectedVersion = events.Min(x => x.Item1.Version) - 1;
            var revision = new StreamRevision((ulong)expectedVersion);

            var eventData = events.Select(x => x.ToEventData());

            var result = await _eventStoreClient.ConditionalAppendToStreamAsync(streamName, revision, eventData, cancellationToken: cancellationToken);

            if (result.Status == ConditionalWriteStatus.VersionMismatch)
            {
                throw new Exception();
            }
        }

        public IAsyncEnumerable<(IEventWrapper, IMetadata)> Get(IEventFilter? eventFilter = null, CancellationToken cancellation = default)
        {
            eventFilter ??= EventFilter.None;
            var eventTypes = eventFilter.EventTypes.ToArray();

            var resolvedEvents = eventFilter.AggregateId.HasValue
                ? GetStream(eventFilter.AggregateId.Value, eventFilter.Checkpoint, cancellation)
                : GetAll(eventFilter, cancellation);

            return ToEventWrapper(resolvedEvents, eventTypes, cancellation);
        }

        private IAsyncEnumerable<ResolvedEvent> GetAll(IEventFilter eventFilter, CancellationToken cancellation)
        {
            IAsyncEnumerable<ResolvedEvent> resolvedEvents;
            var filter = eventFilter.ToGESFilter();
            var allPosition = GetPosition(eventFilter.Checkpoint);

            resolvedEvents = _eventStoreClient.ReadAllAsync(
                direction: Direction.Forwards,
                position: allPosition,
                maxCount: int.MaxValue,
                filter: filter,
                cancellationToken: cancellation,
                resolveLinkTos: true,
                userCredentials: _userCredentials
            );

            return resolvedEvents;
        }

        private IAsyncEnumerable<ResolvedEvent> GetStream(NonEmptyGuid id, long? checkpoint, CancellationToken cancellation) =>
            _eventStoreClient.ReadStreamAsync(
                direction: Direction.Forwards,
                streamName: id.ToString(),
                revision: GetRevision(checkpoint),
                count: int.MaxValue,
                resolveLinkTos: true,
                cancellationToken: cancellation,
                userCredentials: _userCredentials
            );

        private async IAsyncEnumerable<(IEventWrapper, IMetadata)> ToEventWrapper(
            IAsyncEnumerable<ResolvedEvent> input,
            string[] eventTypes,
            [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var enumerator = input.GetAsyncEnumerator();
            bool hasResult;
            try
            {
                hasResult = await enumerator.MoveNextAsync(cancellationToken);
            }
            catch (StreamNotFoundException)
            {
                yield break;
            }

            while(hasResult)
            {
                var wrapper = enumerator.Current.Deserialize(eventTypes);
                if (wrapper.HasValue)
                {
                    yield return wrapper.Value;
                }

                hasResult = await enumerator.MoveNextAsync(cancellationToken);
            }
        }

        public IAsyncEnumerable<(IEventWrapper, IMetadata)> Subscribe(IEventFilter? eventFilter, CancellationToken cancellation)
        {
            eventFilter ??= EventFilter.None;
            var channel = Channel.CreateUnbounded<(IEventWrapper, IMetadata)>();
            
            if (eventFilter.AggregateId.HasValue)
            {
                SubscribeToStream(eventFilter.AggregateId.Value, eventFilter.Checkpoint, eventFilter.EventTypes.ToArray(), channel.Writer, cancellation);
            }
            else
            {
                SubscribeToAll(eventFilter, channel.Writer, cancellation);
            }

            return channel.Reader.ReadAllAsync(cancellation);
        }

        private void SubscribeToAll(IEventFilter eventFilter, ChannelWriter<(IEventWrapper, IMetadata)> writer, CancellationToken cancellation)
        {
            var position = GetPosition(eventFilter.Checkpoint);
            var filter = eventFilter.ToGESFilter();
            var eventTypes = eventFilter.EventTypes;

            _eventStoreClient.SubscribeToAll(
                start: position,
                eventAppeared: (_, e, c) => EventAppeared(e, writer, eventTypes.ToArray(), c),
                resolveLinkTos: true,
                cancellationToken: cancellation,
                userCredentials: _userCredentials
            );
        }

        private void SubscribeToStream(NonEmptyGuid id, long? checkpoint, string[] eventTypes, ChannelWriter<(IEventWrapper, IMetadata)> writer, CancellationToken cancellation)
        {
            var streamRevision = GetRevision(checkpoint);

            _eventStoreClient.SubscribeToStream(
                streamName: id.ToString(),
                start: streamRevision,
                eventAppeared: (_, e, c) => EventAppeared(e, writer, eventTypes, c),
                resolveLinkTos: true,
                cancellationToken: cancellation,
                userCredentials: _userCredentials
            );
        }

        private Task EventAppeared(ResolvedEvent resolvedEvent, ChannelWriter<(IEventWrapper, IMetadata)> writer, string[] eventTypes, CancellationToken cancellationToken)
        {
            var wrapper = resolvedEvent.Deserialize(eventTypes);
            if (wrapper.HasValue)
            {
                return writer.PublishAsync(wrapper.Value, cancellationToken).AsTask();
            }

            return Task.CompletedTask;
        }


        private Position GetPosition(long? checkpoint)
        {
            return checkpoint.HasValue
                ? new Position((ulong)checkpoint, (ulong)checkpoint)
                : Position.Start;
        }

        private StreamRevision GetRevision(long? checkpoint) => checkpoint.HasValue
                    ? StreamRevision.FromInt64(checkpoint.Value)
                    : StreamRevision.Start;
    }
}
