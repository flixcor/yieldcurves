using System;
using System.Collections.Generic;

namespace Common.EventStore.Lib
{
    internal struct EventFilterInstance : ICanAddAggregate, ICanAddEventTypes, ICanAddCheckpoint
    {
        public long? Checkpoint { get; private set; }
        public Guid? AggregateId { get; private set; }
        public IEnumerable<string> EventTypes { get; private set; }

        public ICanAddEventTypes ForAggregate(Guid id)
        {
            AggregateId = id;
            return this;
        }

        public IEventFilter ForEventTypes(params string[] eventTypes)
        {
            EventTypes = eventTypes;

            return this;
        }

        public ICanAddAggregate FromCheckpoint(long checkpoint)
        {
            Checkpoint = checkpoint;
            return this;
        }
    }

    public static class EventFilter
    {
        public static readonly IEventFilter None = new EventFilterInstance();
        public static ICanAddAggregate FromCheckpoint(long checkpoint) => new EventFilterInstance().FromCheckpoint(checkpoint);
        public static ICanAddEventTypes ForAggregate(Guid id) => new EventFilterInstance().ForAggregate(id);
        public static IEventFilter ForEventTypes(params string[] eventTypes) => new EventFilterInstance().ForEventTypes(eventTypes);
    }
}
