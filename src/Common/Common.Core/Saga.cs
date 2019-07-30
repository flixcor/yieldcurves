using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Core
{
    public abstract class Saga<TSaga> : Aggregate<TSaga> where TSaga : Saga<TSaga>
    {
        private static readonly IDictionary<Type, Action<Saga<TSaga>, Event>> s_actions = new Dictionary<Type, Action<Saga<TSaga>, Event>>();
        protected IList<Event> TransitionedEvents { get; } = new List<Event>();

        static Saga()
        {
            RegisterApplyMethod<TransitionedEvent>(Apply);
        }

        protected static void RegisterTransitionMethod<TEvent>(Action<TSaga, TEvent> action) where TEvent : Event
        {
            s_actions.Add(typeof(TEvent), (x, y) => action((TSaga)x, (TEvent)y));
        }

        public void TransitionEvent(Event @event)
        {
            if (!TransitionedEvents.Any(x => x.Equals(@event)))
            {
                ApplyEvent(new TransitionedEvent(Id, @event));

                if (s_actions.TryGetValue(@event.GetType(), out var action))
                {
                    action(this, @event);
                }
            }
        }

        private static void Apply(Saga<TSaga> s, TransitionedEvent e)
        {
            var innerEvent = e.InnerEvent;
            s.TransitionedEvents.Add(innerEvent);
        }
    }
}
