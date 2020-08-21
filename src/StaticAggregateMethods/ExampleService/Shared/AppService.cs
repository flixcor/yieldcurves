using System;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public abstract record AppService<T> : ICommand where T : IAggregateState, new()
    {
        protected abstract string GetId();

        public async Task Handle()
        {
            var store = RestMapper.GetAggregateStore();
            var aggregate = await store.Load<T>(GetId());
            await store.Save(await HandleInternal(aggregate));
        }

        protected abstract ValueTask<T> HandleInternal(T aggregate);
    }
}
