using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IAggregateRepository
    {
        Task<T> Load<T>(NonEmptyGuid id, CancellationToken cancellationToken = default) where T : Aggregate<T>, new();
        Task Save<T>(T aggregate, CancellationToken cancellationToken = default, NonEmptyGuid? causationId = null, NonEmptyGuid? correlationId = null) where T : Aggregate<T>, new();
    }
}
