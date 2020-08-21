using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface IAggregateStore
    {
        Task<T> Load<T>(string id, CancellationToken token = default) where T : IAggregateState, new();
        Task Save<T>(T aggregate, CancellationToken token = default) where T : IAggregateState, new();
    }
}
