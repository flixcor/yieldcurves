using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Proto;
using EventStore.Client;

namespace Common.Infrastructure
{
    internal class EventStoreRepository : IRepository
    {
        private readonly EventStoreClient _client;

        public EventStoreRepository(EventStoreClient client)
        {
            _client = client;
        }

        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate<TAggregate>
        {
            var streamName = AggregateIdToStreamName<TAggregate>(id);
            var aggregate = ConstructAggregate<TAggregate>();

            var events = _client.ReadStreamAsync(Direction.Forwards, streamName, StreamRevision.Start, ulong.MaxValue);

            var found = false;

            await foreach (var item in events)
            {
                found = true;
                var history = item.Deserialize().Item3;
                aggregate.LoadStateFromHistory(new[] { history });
            }

            return found
                ? aggregate
                : null;
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate<TAggregate>
        {
            var aggregateName = aggregate.GetType().Name;

            var streamName = AggregateIdToStreamName<TAggregate>(aggregate.Id);
            var eventsToPublish = aggregate.GetUncommittedEvents();
            var newEvents = eventsToPublish.ToList();
            var originalVersion = aggregate.Version - newEvents.Count;
            var expectedVersion = originalVersion == -1 ? StreamRevision.Start : StreamRevision.FromInt64(originalVersion);
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, aggregate.Id, aggregateName)).ToList();

            await _client.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);

            aggregate.MarkEventsAsCommitted();

        }

        private TAggregate ConstructAggregate<TAggregate>() where TAggregate : Aggregate<TAggregate>
        {
            var agg = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            return agg;
        }

        private string AggregateIdToStreamName<T>(Guid id) where T : Aggregate<T>
        {
            var name = typeof(T).Name;
            //Ensure first character of type name is lower case to follow javascript naming conventions
            return string.Format("{0}-{1}", char.ToLower(name[0]) + name.Substring(1), id.ToString("N"));
        }

        private static EventData ToEventData(Guid eventId, IEvent @event, Guid aggregateId, string aggregateName)
        {
            var data = Serializer.Serialize(@event);
            var eventName = @event.GetType().Name;

            var eventHeaders = new EventHeaders
            {
                CommitId = aggregateId,
                AggregateName = aggregateName,
                EventName = eventName
            };

            var metadata = Serializer.Serialize(eventHeaders);

            return new EventData(Uuid.FromGuid(eventId), eventName, data, metadata);
        }
    }
}
