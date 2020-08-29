using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lib.EventSourcing
{
    public class InMemoryProjectionStore
    {
        public static readonly InMemoryProjectionStore Instance = new InMemoryProjectionStore();

        private InMemoryProjectionStore() { }

        private readonly ConcurrentDictionary<Type, (long, Dictionary<string, object>)> _projections = new ConcurrentDictionary<Type, (long, Dictionary<string, object>)>();

        public (long, T) Get<T>(string id) where T : class =>
            _projections.TryGetValue(typeof(T), out var projection)
                ? projection.Item2.TryGetValue(id, out var obj) && obj is T t
                    ? (projection.Item1, t)
                    : (projection.Item1, null)
                : (0, null);

        public (long, IEnumerable<T>) GetAll<T>() => _projections.TryGetValue(typeof(T), out var projection)
            ? (projection.Item1, projection.Item2.Values.OfType<T>())
            : (0, Enumerable.Empty<T>());

        public void AddOrUpdate<T>(long version, Func<T, T> mapper, string id) where T : class, new () 
        => _projections.AddOrUpdate(
            typeof(T),
            _ => (version, new Dictionary<string, object> { { id, mapper(new T()) } }),
            (_, tup) => version > tup.Item1 
                ? (version, new Dictionary<string, object>(tup.Item2) { [id] = mapper(tup.Item2.TryGetValue(id, out var val) && val is T t ? t : new T()) })
                : tup
            );

        public void Delete<T>(long version, string id) => _projections.AddOrUpdate(
            typeof(T),
            _ => (version, new Dictionary<string, object>()),
            (_, tup) => version > tup.Item1
                ? (version, new Dictionary<string, object>(tup.Item2) { [id] = null })
                : tup
            );
    }
}
