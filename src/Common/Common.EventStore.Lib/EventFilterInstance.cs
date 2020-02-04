using System;
using System.Collections.Generic;

namespace Common.EventStore.Lib
{
    internal class EventFilterInstance : ICanAddAggregate, ICanAddEventTypes, ICanAddCheckpoint
    {
        private readonly HashSet<string> _eventTypes = new HashSet<string>();

        public long? Checkpoint { get; private set; }
        public Guid? AggregateId { get; private set; }
        public IEnumerable<string> EventTypes { get => _eventTypes; }

        public ICanAddEventTypes ForAggregate(Guid id)
        {
            AggregateId = id;
            return this;
        }

        public IEventFilter ForEventTypes(params string[] eventTypes)
        {
            foreach (var item in eventTypes)
            {
                _eventTypes.Add(item);
            }

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
