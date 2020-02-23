using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

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

        public async Task<Either<Error, Nothing>> Handle(CancellationToken cancelationToken, NonEmptyGuid id, Action<TAggregate> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            whatToDo(aggregate);

            await _aggregateStore.Save(aggregate, cancelationToken);

            return Nothing.Instance;
        }

        public async Task<Either<Error, Nothing>> Handle(CancellationToken cancelationToken, NonEmptyGuid id, Func<TAggregate, TAggregate> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            await _aggregateStore.Save(whatToDo(aggregate), cancelationToken);

            return Nothing.Instance;
        }

        public async Task<Either<Error, Nothing>> Handle(CancellationToken cancelationToken, NonEmptyGuid id, Func<TAggregate, Either<Error, Nothing>> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            return await
                whatToDo(aggregate)
                .MapRight(_ => _aggregateStore.Save(aggregate, cancelationToken))
                .Reduce();
        }

        public async Task<Either<Error, Nothing>> Handle(CancellationToken cancelationToken, NonEmptyGuid id, Func<TAggregate, Either<Error, TAggregate>> whatToDo)
        {
            var aggregate = await _aggregateStore.Load<TAggregate>(id, cancelationToken);

            return await
                whatToDo(aggregate)
                .MapRight(a => _aggregateStore.Save(a, cancelationToken))
                .Reduce();
        }
    }
}
