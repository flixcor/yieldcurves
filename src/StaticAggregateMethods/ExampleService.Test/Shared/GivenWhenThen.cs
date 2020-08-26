using System;
using System.Linq;
using Lib.Aggregates;
using Xunit;

namespace Lib.Test.Shared
{
    public interface IWhen<S>
    {
        IThen<S, T> When<T>(T command);
    }

    public interface IThen<S, C>
    {
        void Then(params object[] events);
    }

    public static class GivenWhenThen<Aggregate, State> where Aggregate : IAggregate<State>, new() where State : class, new()
    {
        static readonly Aggregate s_aggregate = new Aggregate();

        private class ThenImpl<C> : IThen<State, C>
        {
            public State State { get; init; }
            public C Command { get; init; }

            public void Then(params object[] events)
            {
                var assertions = s_aggregate.Handle(State, Command).Select(GetAssertion).ToArray();
                Assert.Collection(events, assertions);
            }

            private static Action<object> GetAssertion(object actual) => (object expected) => Assert.Equal(expected, actual);
        }

        public static IWhen<State> Given(params object[] events)
            => new WhenImpl { Events = events };
        public static IThen<State, C> When<C>(C command) => new ThenImpl<C> { State = new State(), Command = command };

        private static readonly object[] s_empty = Array.Empty<object>();

        private record WhenImpl : IWhen<State>
        {
            public object[] Events { get; init; } = s_empty;

            IThen<State, Command> IWhen<State>.When<Command>(Command command)
            {
                var state = Events.Aggregate(new State(), (s, e) => s_aggregate.When(s, e));

                return new ThenImpl<Command>
                {
                    Command = command,
                    State = state
                };
            }
        }
    }
}
