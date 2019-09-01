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

        public async Task<Maybe<T>> Get(Guid id) => (await _db.FindAsync<T>(id)).Return();

        public async Task<IEnumerable<T>> GetAll() => await _db.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where) => await _db.Set<T>().Where(where).ToListAsync();

        public Task Insert(T t)
        {
            _db.Add(t);
            return _db.SaveChangesAsync();
        }

        public async Task<Maybe<T>> Single(Expression<Func<T, bool>> where) => (await _db.Set<T>().Where(where).FirstOrDefaultAsync()).Return();

        public Task Update(T t)
        {
            _db.Update(t);
            return _db.SaveChangesAsync();
        }
    }
}
