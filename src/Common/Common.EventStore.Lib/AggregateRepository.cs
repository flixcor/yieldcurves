using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.EventStore.Lib.EfCore
{
    internal class AggregateRepository : IAggregateRepository
    {
        private readonly IEventRepository _eventRepository;

        public AggregateRepository(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : Aggregate<T>
        {
            var aggregate = (T?)Activator.CreateInstance(typeof(T), true) ?? throw new Exception();
            aggregate.Id = id;

            var loaded = false;

            var filter = EventFilter.ForAggregate(id);

            await foreach (var item in _eventRepository.Get(filter, cancellationToken))
            {
                loaded = true;
                aggregate.LoadFromHistory(item);
            }

            return loaded
                ? aggregate
                : null;
        }

        public Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken = default) where T : Aggregate<T>
        {
            var uncommitted = aggregate.GetUncommittedEvents().ToArray();
            return _eventRepository.Save(cancellationToken, uncommitted);
        }
    }
}
