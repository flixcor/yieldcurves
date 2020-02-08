using Common.Core;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class RedisReadModelRepository<T> : IReadModelRepository<T> where T : ReadObject
    {
        private readonly IDatabase _db;

        public RedisReadModelRepository(IDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<T?> Single(Expression<Func<T, bool>> where)
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            var readObject = result
                .Select(v => JsonConvert.DeserializeObject<T>(v))
                .AsQueryable()
                .FirstOrDefault(where);

            return readObject;
        }

        public async Task<T?> Get(NonEmptyGuid id)
        {
            T? dto = null;

            var key = Key(id);
            var result = await _db.StringGetAsync(key);

            if (result.HasValue)
            {
                dto = JsonConvert.DeserializeObject<T>(result);
            }

            return dto;
        }

        public async IAsyncEnumerable<T> GetAll()
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            foreach (var item in result)
            {
                yield return JsonConvert.DeserializeObject<T>(item);
            }
        }

        public async IAsyncEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            var readObjects = result.Select(v => JsonConvert.DeserializeObject<T>(v)).AsQueryable().Where(where);

            foreach (var item in readObjects)
            {
                yield return item;
            }
        }

        public async Task Insert(T t)
        {
            var serialised = JsonConvert.SerializeObject(t);
            var key = Key(t.Id);
            var transaction = _db.CreateTransaction();
            var setTask = transaction.StringSetAsync(key, serialised);
            var addTask = transaction.SetAddAsync(SetName(), t.Id.ToString("N"));

            if (await transaction.ExecuteAsync())
            {
                await setTask;
                await addTask;
            }
        }

        public Task Update(T t)
        {
            var key = Key(t.Id);
            var serialised = JsonConvert.SerializeObject(t);
            return _db.StringSetAsync(key, serialised, when: When.Exists);
        }

        private string Key(NonEmptyGuid id)
        {
            return InstanceName() + id.ToString("N");
        }

        private static string InstanceName()
        {
            var type = typeof(T);
            return string.Format("{0}:", type.FullName);
        }

        private string SetName()
        {
            return string.Format("{0}Set", InstanceName());
        }
    }
}
