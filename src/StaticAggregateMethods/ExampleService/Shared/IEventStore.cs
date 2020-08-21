using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public interface IEventStore
    {
        Task Save(string stream, CancellationToken cancellationToken = default, params EventWrapper[] events);
        IAsyncEnumerable<EventWrapper> Get(string stream, CancellationToken cancellation = default);
        IAsyncEnumerable<EventWrapper> Subscribe(CancellationToken cancellation);
    }
}
