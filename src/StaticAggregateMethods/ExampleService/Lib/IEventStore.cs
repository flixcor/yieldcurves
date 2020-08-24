using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface IEventStore
    {
        Task Save(string stream, CancellationToken cancellationToken = default, params EventEnvelope[] events);
        IAsyncEnumerable<EventEnvelope> Get(string stream, CancellationToken cancellation = default);
        IAsyncEnumerable<EventEnvelope> Subscribe(CancellationToken cancellation);
    }
}
