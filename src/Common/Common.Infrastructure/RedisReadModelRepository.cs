﻿using Common.Core;
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

        public async Task<Maybe<T>> Single(Expression<Func<T, bool>> where)
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            var readObject = result
                .Select(v => JsonConvert.DeserializeObject<T>(v))
                .AsQueryable()
                .FirstOrDefault(where);

            return readObject.Return();
        }

        public async Task<Maybe<T>> Get(Guid id)
        {
            T dto = null;

            var key = Key(id);
            var result = await _db.StringGetAsync(key);

            if (result.HasValue)
            {
                dto = JsonConvert.DeserializeObject<T>(result);
            }

            return dto.Return();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            var readObjects = result.Select(v => JsonConvert.DeserializeObject<T>(v)).AsEnumerable();
            return readObjects.ToList();
        }

        public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where)
        {
            var get = new RedisValue[] { InstanceName() + "*" };
            var result = await _db.SortAsync(SetName(), sortType: SortType.Alphabetic, by: "nosort", get: get);

            var readObjects = result.Select(v => JsonConvert.DeserializeObject<T>(v)).AsQueryable().Where(where);
            return readObjects.ToList();
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

        private string Key(Guid id)
        {
            return InstanceName() + id.ToString("N");
        }

        private string InstanceName()
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