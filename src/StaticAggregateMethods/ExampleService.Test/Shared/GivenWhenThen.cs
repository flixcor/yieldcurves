using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using static ExampleService.Shared.Handlers;

namespace ExampleService.Test.Shared
{
    public interface IGiven
    {
        IWhen Given(params object[] events);
    }

    public interface IWhen
    {
        IThen When(params object[] commands);
    }

    public interface IThen
    {
        void Then(params object[] events);
    }

    public static class GivenWhenThen<State> where State : class, new()
    {
        public static IWhen Given(params object[] events) => (new GivenWhenThenImpl() as IGiven).Given(events);
        public static IThen When(params object[] commands) => (new GivenWhenThenImpl() as IWhen).When(commands);

        static GivenWhenThen()
        {
            Setup();
        }

        private static readonly Dictionary<Type, Func<object, State, IEnumerable<object>>> s_commandHandlers = new Dictionary<Type, Func<object, State, IEnumerable<object>>>();
        private static readonly Dictionary<Type, Func<object, State, State>> s_eventProcessors = new Dictionary<Type, Func<object, State, State>>();

        private static void Setup()
        {
            var typeofT = typeof(State);
            var commandHandlerType = typeof(CommandHandler<,>);
            var eventApplierType = typeof(EventApplier<,>);
            var delegateTypes = new[] { commandHandlerType, eventApplierType };

            var handlerTypes = typeof(State).Assembly.GetTypes().SelectMany(x => x.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x =>
                    x.FieldType.IsGenericType
                    && delegateTypes.Contains(x.FieldType.GetGenericTypeDefinition())
                    && x.FieldType.GetGenericArguments().Last() == typeofT)
                );
            foreach (var item in handlerTypes)
            {
                var value = item.GetValue(null) as Delegate;
                var firstArgument = item.FieldType.GetGenericArguments().First();

                if (item.FieldType.GetGenericTypeDefinition() == commandHandlerType)
                {
                    s_commandHandlers[firstArgument] = (command, state) => value.DynamicInvoke(new[] { command, state }) as IEnumerable<object>;
                    continue;
                }

                s_eventProcessors[firstArgument] = (@event, state) => value.DynamicInvoke(new[] { @event, state }) as State;
            }
        }

        private static readonly object[] s_empty = Array.Empty<object>();

        private record GivenWhenThenImpl : IGiven, IWhen, IThen
        {
            private object[] Events { get; init; } = s_empty;
            private object[] Commands { get; init; } = s_empty;

            IWhen IGiven.Given(params object[] events) => this with { Events = events };

            IThen IWhen.When(params object[] commands) => this with { Commands = commands };

            void IThen.Then(params object[] expectedEvents)
            {
                var outputEvents = new List<object>();
                var initialState = Events.Aggregate(new State(), Fold);

                Commands.Aggregate(initialState, (state, command) =>
                {
                    var events = s_commandHandlers[command.GetType()].Invoke(command, state).ToList();
                    outputEvents.AddRange(events);
                    return events.Aggregate(state, Fold);
                });

                var assertions = outputEvents.Select(GetAssertion).ToArray();
                Assert.Collection(expectedEvents, assertions);
            }

            private static State Fold(State state, object @event) => s_eventProcessors[@event.GetType()].Invoke(@event, state);
            private static Action<object> GetAssertion(object actual) => (object expected) => Assert.Equal(expected, actual);
        }
    }
}
