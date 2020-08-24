using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public static class InMemoryProjectionStore
    {
        private static Dictionary<Type, (object State, Func<object, EventEnvelope, object> Projection)> _projections = new Dictionary<Type, (object, Func<object, EventEnvelope, object>)>();

        public static bool TryRegister<T>(Func<T, EventEnvelope, T> projection) where T : class, new()
        {
            var type = typeof(T);

            if (_projections.ContainsKey(type))
            {
                return false;
            }

            _projections[type] = (new T(), (state, e) => Convert(state, e, projection));

            return true;
        }

        public static Task<T> GetAsync<T>() where T : class, new() => Task.FromResult(_projections[typeof(T)].Item1 as T);

        public static void Project(EventEnvelope eventWrapper) => _projections = _projections.ToDictionary(x => x.Key, x => (x.Value.Projection(x.Value.State, eventWrapper), x.Value.Projection));

        private static object Convert<T>(object state, EventEnvelope @event, Func<T, EventEnvelope, T> projection) where T : class, new() => projection(state as T, @event);
    }
}
