using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.EventSourcing
{
    public interface IEventStore
    {
        Task<long?> Save(string stream, CancellationToken cancellationToken = default, params EventEnvelope[] events);
        IAsyncEnumerable<EventEnvelope> Get(string stream, CancellationToken cancellation = default);
        IAsyncEnumerable<EventEnvelope> Subscribe(CancellationToken cancellation);
    }
}
