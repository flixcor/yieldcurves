using System.Collections.Generic;
using Common.Core;

namespace Common.EventStore.Lib
{
    public interface IEventFilter
    {
        long? Checkpoint { get; }
        NonEmptyGuid? AggregateId { get; }
        ICollection<string> EventTypes { get; }
    }

    public interface ICanAddCheckpoint : IEventFilter
    {
        public ICanAddAggregate FromCheckpoint(long checkpoint);
    }

    public interface ICanAddAggregate : IEventFilter
    {
        public ICanAddEventTypes ForAggregate(NonEmptyGuid id);
    }

    public interface ICanAddEventTypes : IEventFilter
    {
        public IEventFilter ForEventTypes(params string[] eventTypes);
    }
}
