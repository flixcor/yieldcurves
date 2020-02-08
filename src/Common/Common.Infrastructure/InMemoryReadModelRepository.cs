using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.Infrastructure
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadObject
    {
        public InMemoryReadModelRepository(IEnumerable<T>? checkpoint = null)
        {
            _checkpoint = checkpoint ?? Enumerable.Empty<T>();
        }

        private IEnumerable<T> _checkpoint;

        public Task<T?> Get(NonEmptyGuid id)
        {
            var readObject = (T?)_checkpoint.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(readObject);
        }

        public IAsyncEnumerable<T> GetAll()
        {
            return new FakeAsyncEnumerable<T>(_checkpoint);
        }

        public IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            var enumerable = _checkpoint.AsQueryable().Where(where);
            return new FakeAsyncEnumerable<T>(enumerable);
        }

        public Task Insert(T t)
        {
            _checkpoint = _checkpoint.Concat(new T[] { t });
            return Task.CompletedTask;
        }

        public Task<T?> Single(Expression<Func<T, bool>> where)
        {
            var readObject = (T?)_checkpoint.AsQueryable().FirstOrDefault(where);
            return Task.FromResult(readObject);
        }

        public Task Update(T t)
        {
            _checkpoint = _checkpoint.Select(x=> x.Id == t.Id? t : x).ToList();
            return Task.CompletedTask;
        }
    }

    internal class FakeAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public FakeAsyncEnumerable(IEnumerable<T> enumerable) 
        {
            _enumerable = enumerable;
        } 

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestDbAsyncEnumerator<T>(_enumerable.GetEnumerator()); 
        }
    } 

    internal class TestDbAsyncEnumerator<T> : IAsyncEnumerator<T>
    { 
        private readonly IEnumerator<T> _inner; 

        public TestDbAsyncEnumerator(IEnumerator<T> inner) 
        { 
            _inner = inner; 
        } 

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }

        public T Current 
        { 
            get { return _inner.Current; } 
        } 
    } 
}
