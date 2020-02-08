using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using static Common.Events.Helpers;

namespace Common.EventStore.Lib.Internal
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

        public async Task<T> Load<T>(NonEmptyGuid id, CancellationToken cancellationToken) where T : Aggregate<T>, new()
        {
            var aggregate = new T
            {
                Id = id
            };

            var filter = EventFilter.ForAggregate(id);

            await foreach (var (item, _) in _readRepository.Get(filter, cancellationToken))
            {
                aggregate.LoadFromHistory(item);
            }

            return aggregate;
        }

        public Task Save<T>(T aggregate, CancellationToken cancellationToken = default, NonEmptyGuid? causationId = null, NonEmptyGuid? correlationId = null) where T : Aggregate<T>, new()
        {
            causationId ??= NonEmptyGuid.New();
            correlationId ??= causationId;

            var uncommitted = aggregate.GetUncommittedEvents().Select(x =>
            {
                var id = Guid.NewGuid().NonEmpty();

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
