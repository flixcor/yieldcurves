﻿using System;
using System.Collections.Generic;

namespace Lib
{
    public interface IAggregate
    {
    }

    public interface IAggregate<State>: IAggregate where State : class, new()
    {
        State When(State state, object @event);
        IEnumerable<object> Handle(State state, object command);
    }

    public abstract class Aggregate<State> : IAggregate<State> where State : class, new()
    {
        public Dictionary<Type, Func<State, object, State>> EventHandlers { get; } = new Dictionary<Type, Func<State, object, State>>();
        public Dictionary<Type, Func<State, object, IEnumerable<object>>> CommandHandlers { get; } = new Dictionary<Type, Func<State, object, IEnumerable<object>>>();

        public Delegates.GetStreamName GetStreamName { get; private set; } = (id) => typeof(State).Name.ToLowerInvariant() + "-" + id;

        protected void When<Event>(Delegates.When<State, Event> func)
            => EventHandlers[typeof(Event)] = (state, @event) => func(state, (Event)@event);

        protected void Handle<Command>(Delegates.Handle<State, Command> handler)
            => CommandHandlers[typeof(Command)] = (state, command) => handler(state, (Command)command);

        protected void Handle<CommandType>(Func<State, CommandType, object> handler) => Handle<CommandType>((s, c) => new[] { handler(s, c) });

        protected void StreamName(Delegates.GetStreamName func) => GetStreamName = func;

        State IAggregate<State>.When(State state, object @event)
        {
            return EventHandlers.TryGetValue(@event.GetType(), out var when)? when(state, @event) : state;
        }

        IEnumerable<object> IAggregate<State>.Handle(State state, object command)
        {
            return CommandHandlers[command.GetType()](state, command);
        }
    }
}
