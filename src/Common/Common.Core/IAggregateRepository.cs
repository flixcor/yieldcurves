using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IAggregateRepository
    {
        Task<T> Load<T>(NonEmptyGuid id, CancellationToken cancellationToken = default) where T : IAggregate, new();
        Task Save<T>(T aggregate, CancellationToken cancellationToken = default, NonEmptyGuid? causationId = null, NonEmptyGuid? correlationId = null) where T : IAggregate, new();
    }
}
