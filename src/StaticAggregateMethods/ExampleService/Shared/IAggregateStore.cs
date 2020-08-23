using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface IAggregateStore
    {
        Task<StateEnvelope<T>> Load<T>(string streamName, CancellationToken token = default) where T: new();
        Task Save<T>(StateEnvelope<T> aggregate, CancellationToken token = default) where T : new();
    }
}
