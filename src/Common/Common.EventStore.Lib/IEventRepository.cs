using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IEventWriteRepository
    {
        Task Save(CancellationToken cancellationToken = default, params (IEventWrapper, IMetadata)[] events);
    }

    public interface IEventReadRepository
    {
        IAsyncEnumerable<(IEventWrapper, IMetadata)> Get(IEventFilter? eventFilter = null, CancellationToken cancellation = default);
        IAsyncEnumerable<(IEventWrapper, IMetadata)> Subscribe(IEventFilter? eventFilter = null, CancellationToken cancellation = default);
    }
}
