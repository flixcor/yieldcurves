using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IReadModelRepository<T>
        where T : ReadObject
    {
        IAsyncEnumerable<T> GetAll();
        IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        Task<Maybe<T>> Single(Expression<Func<T, bool>> where);
        Task<Maybe<T>> Get(Guid id);
        Task Update(T t);
        Task Insert(T t);
    }
}
