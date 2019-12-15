using System;
using System.Collections.Generic;
using System.Linq;

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

        public void LoadStateFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history)
            {
                ApplyEvent(e, false);
            }
        }

        protected void ApplyEvent(IEvent @event)
        {
            ApplyEvent(@event, true);
        }

        private void ApplyEvent(IEvent @event, bool isNew)
        {
            var interfaces = @event
                .GetType()
                .GetInterfaces()
                .Where(i => i.GetInterface(nameof(IEvent)) != null);

            var actions = s_actions
                .Where(d => interfaces.Contains(d.Key))
                .Select(x => x.Value);

            foreach (var action in actions)
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
