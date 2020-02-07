using System;
using System.Collections.Generic;

namespace Common.EventStore.Lib
{
    internal struct EventFilterInstance : ICanAddAggregate, ICanAddEventTypes, ICanAddCheckpoint
    {
        private EventFilterInstance(long? checkpoint, Guid? aggregateId, ICollection<string> eventTypes)
        {
            Checkpoint = checkpoint;
            AggregateId = aggregateId;
            EventTypes = eventTypes;
        }

        public long? Checkpoint { get; }
        public Guid? AggregateId { get; }
        public ICollection<string> EventTypes { get; }

        public ICanAddEventTypes ForAggregate(Guid id) => new EventFilterInstance(Checkpoint, id, EventTypes);

        public IEventFilter ForEventTypes(params string[] eventTypes) => new EventFilterInstance(Checkpoint, AggregateId, eventTypes);

        public ICanAddAggregate FromCheckpoint(long checkpoint) => new EventFilterInstance(checkpoint, AggregateId, EventTypes);
    }

    public static class EventFilter
    {
        public static readonly IEventFilter None = new EventFilterInstance();
        public static ICanAddAggregate FromCheckpoint(long checkpoint) => new EventFilterInstance().FromCheckpoint(checkpoint);
        public static ICanAddEventTypes ForAggregate(Guid id) => new EventFilterInstance().ForAggregate(id);
        public static IEventFilter ForEventTypes(params string[] eventTypes) => new EventFilterInstance().ForEventTypes(eventTypes);
    }
}
