﻿using System;
using System.Collections.Generic;

namespace Common.Core
{
    public abstract class Aggregate<TAggregate> where TAggregate : Aggregate<TAggregate>
    {
        private static readonly IDictionary<Type, Action<Aggregate<TAggregate>, IEvent>> s_actions = new Dictionary<Type, Action<Aggregate<TAggregate>, IEvent>>();
        private readonly List<IEvent> _events = new List<IEvent>();

        protected static void RegisterApplyMethod<TEvent>(Action<TAggregate, TEvent> action) where TEvent : IEvent
        {
            s_actions.Add(typeof(TEvent), (x, y) => action((TAggregate)x, (TEvent)y));
        }

        public Guid Id { get; protected set; }
        public int Version { get; private set; } = -1;

        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _events.AsReadOnly();
        }

        public void MarkEventsAsCommitted()
        {
            _events.Clear();
        }

        public void LoadStateFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                ApplyEvent(e, false);
            }
        }

        protected void ApplyEvent(Event @event)
        {
            ApplyEvent(@event, true);
        }

        private void ApplyEvent(Event @event, bool isNew)
        {
            if (s_actions.TryGetValue(@event.GetType(), out var action))
            {
                action(this, @event);
            }

            if (isNew)
            {
                @event =  @event.WithVersion(++Version);
                _events.Add(@event);
            }
            else
            {
                Version = @event.Version;
            }
        }
    }
}
