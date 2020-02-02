using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IEventRepository
    {
        IAsyncEnumerable<IEventWrapper> GetEvents(Guid aggregateId, CancellationToken cancellation);
        Task SaveEvents(CancellationToken cancellationToken, params IEventWrapper[] events);
    }
}
