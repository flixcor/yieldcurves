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
        public InMemoryReadModelRepository(IEnumerable<T> checkpoint = null)
        {
            _checkpoint = checkpoint ?? Enumerable.Empty<T>();
        }

        private IEnumerable<T> _checkpoint;

        public Task<Maybe<T>> Get(Guid id)
        {
            var readObject = _checkpoint.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(readObject.Maybe());
        }

        public async IAsyncEnumerable<T> GetAll()
        {
            foreach (var item in _checkpoint)
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            foreach (var item in _checkpoint.AsQueryable().Where(where))
            {
                yield return item;
            }
        }

        public Task Insert(T t)
        {
            _checkpoint = _checkpoint.Concat(new T[] { t });
            return Task.CompletedTask;
        }

        public Task<Maybe<T>> Single(Expression<Func<T, bool>> where)
        {
            var readObject = _checkpoint.AsQueryable().FirstOrDefault(where);
            return Task.FromResult(readObject.Maybe());
        }

        public Task Update(T t)
        {
            _checkpoint = _checkpoint.Select(x=> x.Id == t.Id? t : x).ToList();
            return Task.CompletedTask;
        }
    }
}
