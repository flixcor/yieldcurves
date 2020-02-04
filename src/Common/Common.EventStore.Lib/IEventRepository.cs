using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IEventRepository
    {
        IAsyncEnumerable<IEventWrapper> Get(CancellationToken cancellation);
        IAsyncEnumerable<IEventWrapper> Get(IEventFilter eventFilter, CancellationToken cancellation);
        Task Save(CancellationToken cancellationToken, params IEventWrapper[] events);
    }
}
