using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Events.Helpers;

namespace Common.EventStore.Lib.EfCore
{
    internal class AggregateRepository : IAggregateRepository
    {
        private readonly IEventWriteRepository _eventRepository;
        private readonly IEventReadRepository _readRepository;

        public AggregateRepository(IEventWriteRepository eventRepository, IEventReadRepository readRepository)
        {
            _eventRepository = eventRepository;
            _readRepository = readRepository;
        }

        public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : Aggregate<T>
        {
            var aggregate = (T?)Activator.CreateInstance(typeof(T), true) ?? throw new Exception();
            aggregate.Id = id;

            var loaded = false;

            var filter = EventFilter.ForAggregate(id);

            await foreach (var (item, metadata) in _readRepository.Get(filter, cancellationToken))
            {
                loaded = true;
                aggregate.LoadFromHistory(item);
            }

            return loaded
                ? aggregate
                : null;
        }

        public Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken = default, Guid? causationId = null, Guid? correlationId = null) where T : Aggregate<T>
        {
            causationId ??= Guid.NewGuid();
            correlationId ??= causationId;

            var uncommitted = aggregate.GetUncommittedEvents().Select(x => 
            {
                var id = Guid.NewGuid();

                var metaData = CreateMetadata(new Dictionary<string, string>
                {
                    { "id", Guid.NewGuid().ToString()},
                    { "causationId", causationId.ToString()},
                    { "correlationId", correlationId.ToString()}
                });

                causationId = id;

                return (x, metaData);
            }).ToArray();
            return _eventRepository.Save(cancellationToken, uncommitted);
        }
    }
}
