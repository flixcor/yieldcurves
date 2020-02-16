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

        public async Task<T> Load<T>(NonEmptyGuid id, CancellationToken cancellationToken = default) where T : IAggregate, new()
        {
            var aggregate = new T();

            if (aggregate is Aggregate eventSourced)
            {
                eventSourced.Id = id;

                var filter = EventFilter.ForAggregate(id);

                await foreach (var (item, _) in _readRepository.Get(filter, cancellationToken))
                {
                    eventSourced.LoadFromHistory(item);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return aggregate;
        }

        public async Task Save<T>(T aggregate, CancellationToken cancellationToken = default, NonEmptyGuid? causationId = null, NonEmptyGuid? correlationId = null) where T : IAggregate, new()
        {
            if (aggregate is Aggregate a)
            {
                causationId ??= NonEmptyGuid.New();
                correlationId ??= causationId;

                var uncommitted = a.GetUncommittedEvents().Select(x =>
                {
                    var id = Guid.NewGuid().NonEmpty();

                    var metaData = CreateMetadata(new Dictionary<string, string>
                {
                    { "id", Guid.NewGuid().ToString()},
                    { "causationId", causationId.Value.ToString()},
                    { "correlationId", correlationId.Value.ToString()}
                });

                    causationId = id;

                    return (x, metaData);
                }).ToArray();

                await _eventRepository.Save(cancellationToken, uncommitted);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
