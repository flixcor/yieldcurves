using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.EventStore.Lib.EfCore
{
    public interface IAggregateRepository
    {
        Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : Aggregate<T>;
        Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken = default) where T : Aggregate<T>;
    }
}
