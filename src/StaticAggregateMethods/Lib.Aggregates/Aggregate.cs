﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Aggregates
{
    public interface IAggregate<State> where State : class
    {
        State When(State state, object @event);
        IEnumerable<object> Handle(State state, object command);
        string GetStreamName(string id);
        State InitState();
    }

    public abstract class Aggregate<State> : IAggregate<State> where State : class
    { 
        private readonly Dictionary<Type, Func<State, object, State>> _eventHandlers = new Dictionary<Type, Func<State, object, State>>();
        private readonly Dictionary<Type, Func<State, object, IEnumerable<object>>> _commandHandlers = new Dictionary<Type, Func<State, object, IEnumerable<object>>>();

        public abstract State InitState();

        public virtual string GetStreamName(string id) => typeof(State).Name.ToLowerInvariant() + "-" + id;

        protected void When<Event>(Delegates.When<State, Event> func)
            => _eventHandlers[typeof(Event)] = (state, @event) => func(state, (Event)@event);

        protected void Handle<Command>(Delegates.Handle<State, Command> handler)
            => _commandHandlers[typeof(Command)] = (state, command) => handler(state, (Command)command);

        protected void Handle<CommandType>(Func<State, CommandType, object> handler) => Handle<CommandType>((s, c) => new[] { handler(s, c) });

        State IAggregate<State>.When(State state, object @event)
        {
            return _eventHandlers.TryGetValue(@event.GetType(), out var when) 
                ? when(state, @event) 
                : state;
        }

        IEnumerable<object> IAggregate<State>.Handle(State state, object command)
        {
            return _commandHandlers.TryGetValue(command.GetType(), out var handler)
                ? handler(state, command) 
                : Enumerable.Empty<object>();
        }
    }
}
