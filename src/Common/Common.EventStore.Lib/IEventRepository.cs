using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IEventRepository
    {
        IAsyncEnumerable<IEventWrapper> Get(IEventFilter? eventFilter = null, CancellationToken cancellation = default);
        Task Save(CancellationToken cancellationToken = default, params IEventWrapper[] events);
    }
}
