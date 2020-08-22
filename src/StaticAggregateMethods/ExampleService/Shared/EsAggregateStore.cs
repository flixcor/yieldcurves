using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public class EsAggregateStore : IAggregateStore
    {
        private readonly IEventStore _eventStore;

        public EsAggregateStore(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<T> Load<T>(string id, CancellationToken token = default) where T : IAggregateState, new()
        {
            var state = new T();
            var es = Cast(state);
            es.Id = id;

            await foreach (var item in _eventStore.Get(es.StreamName, token))
            {
                state = Cast(state).LoadFromHistory(item);
            }

            return state;
        }

        public Task Save<T>(T aggregate, CancellationToken token = default) where T : IAggregateState, new()
        {
            var es = Cast(aggregate);
            var uncommitted = es.GetUncommittedEvents();
            return _eventStore.Save(es.StreamName, token, uncommitted.ToArray());
        }

        private EsAggregateState<T> Cast<T>(T t) where T : IAggregateState, new() => t is EsAggregateState<T> s ? s : throw new System.Exception();
    }
}
