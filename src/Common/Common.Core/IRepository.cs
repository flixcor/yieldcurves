using System;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IRepository
    {
        Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate<TAggregate>;

        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : Aggregate<TAggregate>;
    }
}
