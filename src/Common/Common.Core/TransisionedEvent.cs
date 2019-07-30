using System;

namespace Common.Core
{
    public class TransitionedEvent : Event
    {
        public TransitionedEvent(Guid id, Event innerEvent) : base(id)
        {
            InnerEvent = innerEvent;
        }

        public Event InnerEvent { get; }
    }
}
