﻿using System;
using System.Collections.Generic;

namespace Common.EventStore.Lib
{
    public interface IEventFilter
    {
        long? Checkpoint { get; }
        Guid? AggregateId { get; }
        ICollection<string> EventTypes { get; }
    }

    public interface ICanAddCheckpoint : IEventFilter
    {
        public ICanAddAggregate FromCheckpoint(long checkpoint);
    }

    public interface ICanAddAggregate : IEventFilter
    {
        public ICanAddEventTypes ForAggregate(Guid id);
    }

    public interface ICanAddEventTypes : IEventFilter
    {
        public IEventFilter ForEventTypes(params string[] eventTypes);
    }
}
