//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Lib
//{
//    public static class Registry
//    {
//        private static readonly Dictionary<Type, Dictionary<Type, Func<object, object, object>>> s_eventHandlers =
//            new Dictionary<Type, Dictionary<Type, Func<object, object, object>>>();

//        private static readonly Dictionary<Type, Dictionary<Type, Func<object, object, IEnumerable<object>>>> s_commandHandlers =
//            new Dictionary<Type, Dictionary<Type, Func<object, object, IEnumerable<object>>>>();

//        private static readonly Dictionary<Type, Delegates.GetStreamName> s_streamNames = new Dictionary<Type, Delegates.GetStreamName>();

//        public static string GetStreamName<State>(string id) => s_streamNames.TryGetValue(typeof(State), out var func)
//            ? func(id)
//            : $"{typeof(State).Name.ToLowerInvariant()}-{id}";

//        public static State When<State>(State state, object @event) where State : class, new()
//        {
//            return s_eventHandlers.TryGetValue(typeof(State), out var agg)
//                && agg.TryGetValue(@event.GetType(), out var handler)
//                && handler?.Invoke(state, @event) is State newState
//                    ? newState
//                    : state;
//        }

//        public static Delegates.Handle<State, Command> GetHandler<State, Command>() => (state, command) => s_commandHandlers[typeof(State)][typeof(Command)](state, command);

//        public static void Register<Aggregate, State>() where Aggregate : Aggregate<State>, new() where State : class, new()
//        {
//            var aggregate = new Aggregate();
//            s_eventHandlers[typeof(State)] = aggregate.EventHandlers;
//            s_commandHandlers[typeof(State)] = aggregate.CommandHandlers;
//            s_streamNames[typeof(State)] = aggregate.GetStreamName;
//        }

//        public static void RegisterAll<T>()
//        {
//            var method = typeof(Registry).GetMethod(nameof(Registry.Register));
//            var aggregate = typeof(Aggregate<>);

//            var mc = typeof(Domain.MarketCurve.Aggregate);

//            var methods = typeof(T).Assembly.GetTypes()
//                .Where(x => x.BaseType != null && !x.BaseType.IsGenericTypeDefinition && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == aggregate)
//                .Select(x =>
//                {
//                    var stateType = x.BaseType.GetGenericArguments().Single();
//                    return method.MakeGenericMethod(x, stateType);
//                }).ToArray();

//            foreach (var item in methods)
//            {
//                item.Invoke(null, null);
//            }
//        }
//    }
//}
