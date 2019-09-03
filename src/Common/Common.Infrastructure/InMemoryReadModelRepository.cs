using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Core;

namespace Common.Infrastructure
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadObject
    {
        public InMemoryReadModelRepository(IList<T> checkpoint = null)
        {
            Checkpoint = checkpoint ?? new List<T>();
        }

        public IList<T> Checkpoint { get; }

        public Task<Maybe<T>> Get(Guid id)
        {
            var readObject = Checkpoint.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(readObject.Maybe());
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return Task.FromResult(Checkpoint.AsEnumerable());
        }

        public Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where)
        {
            return Task.FromResult(Checkpoint.Where(where.Compile()));
        }

        public Task Insert(T t)
        {
            Checkpoint.Add(t);
            return Task.CompletedTask;
        }

        public Task<Maybe<T>> Single(Expression<Func<T, bool>> where)
        {
            var readObject = Checkpoint.FirstOrDefault(where.Compile());

            return Task.FromResult(readObject.Maybe());
        }

        public Task Update(T t)
        {
            var current = Checkpoint.FirstOrDefault(x => x.Id == t.Id);
            Checkpoint.Remove(current);
            Checkpoint.Add(t);

            return Task.CompletedTask;
        }
    }
}
