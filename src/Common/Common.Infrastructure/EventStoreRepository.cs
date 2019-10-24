using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Proto;
using EventStore.ClientAPI;

namespace Common.Infrastructure
{
    internal class EventStoreRepository : IRepository
    {
        private readonly string _connectionString;
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        public EventStoreRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate<TAggregate>
        {
            var version = int.MaxValue;

            var streamName = AggregateIdToStreamName(typeof(TAggregate), id);
            var aggregate = ConstructAggregate<TAggregate>();

            var sliceStart = (long)0;
            StreamEventsSlice currentSlice;

            using (var connection = EventStoreConnection.Create(_connectionString))
            {
                await connection.ConnectAsync();

                do
                {
                    var sliceCount = sliceStart + ReadPageSize <= version
                                        ? ReadPageSize
                                        : version - (int)sliceStart + 1;



                    currentSlice = await connection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, false);

                    //if (currentSlice.Status == StreamNotFound)
                    //{
                    //    return Result.Fail<TAggregate>($"{nameof(StreamNotFound)}, {id}, {typeof(TAggregate)}");
                    //}

                    //if (currentSlice.Status == StreamDeleted)
                    //{
                    //    return Result.Fail<TAggregate>($"{nameof(StreamDeleted)}, {id}, {typeof(TAggregate)}");
                    //}

                    sliceStart = currentSlice.NextEventNumber;

                    var history = currentSlice.Events.Select(x => x.Deserialize());
                    aggregate.LoadStateFromHistory(history);
                } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);
            }



            //if (aggregate.Version != version && version < int.MaxValue)
            //{
            //    return Result.Fail<TAggregate>($"{id}, {typeof(TAggregate)}, {aggregate.Version}, {version}");
            //}

            //if (sliceStart == 0 && !currentSlice.Events.Any())
            //{
            //    Result.Fail<TAggregate>("Not found");
            //}

            //return Result.Ok(aggregate);

            return aggregate;
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate<TAggregate>
        {
            var assemblyQualifiedName = aggregate.GetType().AssemblyQualifiedName;

            var streamName = AggregateIdToStreamName(aggregate.GetType(), aggregate.Id);
            var eventsToPublish = aggregate.GetUncommittedEvents();
            var newEvents = eventsToPublish.ToList();
            var originalVersion = aggregate.Version - newEvents.Count;
            var expectedVersion = originalVersion == -1 ? ExpectedVersion.NoStream : originalVersion;
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, aggregate.Id, assemblyQualifiedName)).ToList();


            using (var connection = EventStoreConnection.Create(_connectionString))
            {
                await connection.ConnectAsync();

                if (eventsToSave.Count < WritePageSize)
                {
                    await connection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave);
                }
                else
                {
                    using var transaction = await connection.StartTransactionAsync(streamName, expectedVersion);
                    var position = 0;
                    while (position < eventsToSave.Count)
                    {
                        var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                        await transaction.WriteAsync(pageEvents);
                        position += WritePageSize;
                    }

                    await transaction.CommitAsync();
                }
            }

            aggregate.MarkEventsAsCommitted();

        }

        private TAggregate ConstructAggregate<TAggregate>() where TAggregate : Aggregate<TAggregate>
        {
            var agg = (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
            return agg;
        }

        private string AggregateIdToStreamName(Type type, Guid id)
        {
            //Ensure first character of type name is lower case to follow javascript naming conventions
            return string.Format("{0}-{1}", char.ToLower(type.Name[0]) + type.Name.Substring(1), id.ToString("N"));
        }

        private static EventData ToEventData(Guid eventId, IEvent @event, Guid aggregateId, string aggregateClrTypeName)
        {
            var data = Serializer.Serialize(@event);

            var eventHeaders = new EventHeaders
            {
                CommitId = aggregateId,
                AggregateClrTypeName = aggregateClrTypeName,
                EventClrTypeName = @event.GetType().AssemblyQualifiedName
            };

            var metadata = Serializer.Serialize(eventHeaders);
            var typeName = @event.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
    }
}
