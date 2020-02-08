using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.EfCore
{
    public class EfCoreRepository<T> : IReadModelRepository<T> where T : ReadObject
    {
        private readonly GenericDbContext _db;

        public EfCoreRepository(GenericDbContext db) => _db = db;

        public async Task<T?> Get(NonEmptyGuid id) => await _db.Set<T>().AsQueryable().FirstOrDefaultAsync(x=> x.Id == id);

        public IAsyncEnumerable<T> GetAll() => _db.Set<T>().AsQueryable().AsAsyncEnumerable();

        public IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where) => _db.Set<T>().AsQueryable().AsAsyncEnumerable();

        public Task Insert(T t)
        {
            _db.Add(t);
            return Task.CompletedTask;
        }

        public async Task<T?> Single(Expression<Func<T, bool>> where) => await _db.Set<T>().FirstOrDefaultAsync(where);

        public Task Update(T t)
        {
            var tracked = _db.Set<T>().Local.FirstOrDefault(x => x.Id == t.Id);

            if (tracked != null)
            {
                _db.Set<T>().Local.Remove(tracked);
            }

            _db.Update(t);
            return Task.CompletedTask;
        }
    }
}
