using System;
using System.Linq;
using ExampleService.Shared;
using Xunit;

namespace ExampleService.Test.Shared
{
    public interface IWhen<S>
    {
        IThen<S, T> When<T>(T command);
    }

    public interface IThen<S, C>
    {
        void Then(params object[] events);
    }

    public static class GivenWhenThen<T>
    {
        static GivenWhenThen()
        {
            Registry.RegisterAll<T>();
        }

        private class ThenImpl<S, C> : IThen<S, C>
        {
            public S State { get; init; }
            public C Command { get; init; }

            public void Then(params object[] events)
            {
                var assertions = Registry.GetHandler<S, C>()(State, Command).Select(GetAssertion).ToArray();
                Assert.Collection(events, assertions);
            }

            private static Action<object> GetAssertion(object actual) => (object expected) => Assert.Equal(expected, actual);
        }

        public static IWhen<S> Given<S>(params object[] events) where S : class, new() 
            => new WhenImpl<S> { Events = events };
        public static IThen<S, C> When<S, C>(C command) where S : class, new() => new ThenImpl<S, C> { State = new S(), Command = command };

        private static readonly object[] s_empty = Array.Empty<object>();

        private record WhenImpl<State> : IWhen<State> where State : class, new()
        {
            public object[] Events { get; init; } = s_empty;

            IThen<State, Command> IWhen<State>.When<Command>(Command command)
            {
                var state = Events.Aggregate(new State(), (s, e) => Registry.When(s, e));

                return new ThenImpl<State, Command>
                {
                    Command = command,
                    State = state
                };
            }
        }
    }
}
