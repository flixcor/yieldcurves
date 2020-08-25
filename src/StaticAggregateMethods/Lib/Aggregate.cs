using System;
using System.Collections.Generic;

namespace Lib
{
    public abstract class Aggregate<State> where State : class, new()
    {
        public Dictionary<Type, Func<object, object, object>> EventHandlers { get; } = new Dictionary<Type, Func<object, object, object>>();
        public Dictionary<Type, Func<object, object, IEnumerable<object>>> CommandHandlers { get; } = new Dictionary<Type, Func<object, object, IEnumerable<object>>>();

        public Delegates.GetStreamName GetStreamName { get; private set; } = (id) => typeof(State).Name.ToLowerInvariant() + "-" + id;

        protected void When<Event>(Delegates.When<State, Event> func)
            => EventHandlers[typeof(Event)] = (state, @event) => func((State)state, (Event)@event);

        protected void Handle<Command>(Delegates.Handle<State, Command> handler)
            => CommandHandlers[typeof(Command)] = (state, command) => handler((State)state, (Command)command);

        protected void Handle<CommandType>(Func<State, CommandType, object> handler) => Handle<CommandType>((s, c) => new[] { handler(s, c) });

        protected void StreamName(Delegates.GetStreamName func) => GetStreamName = func;
    }
}
