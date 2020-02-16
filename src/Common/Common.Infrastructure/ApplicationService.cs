using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;

namespace Common.Infrastructure
{
    public abstract class ApplicationService<TAggregate>
        where TAggregate : IAggregate, new()
    {
        readonly IAggregateRepository _aggregateStore;

        protected ApplicationService(IAggregateRepository aggregateStore)
        {
            _aggregateStore = aggregateStore;
        }

        public async Task Handle(CancellationToken cancelationToken, NonEmptyGuid id, Action<TAggregate> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            whatToDo(aggregate);

            await _aggregateStore.Save(aggregate, cancelationToken);
        }

        public async Task<Result> Handle(CancellationToken cancelationToken, NonEmptyGuid id, Func<TAggregate, Result> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            return await whatToDo(aggregate).Promise(() => _aggregateStore.Save(aggregate, cancelationToken));
        }
    }
}
