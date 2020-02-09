using System;
using System.Collections.Generic;
using Common.Core;
using Common.EventStore.Lib;
using Microsoft.AspNetCore.Mvc;

namespace Common.EventStore
{
    [BindProperties(SupportsGet = true)]
    public class EventRequest : IEventFilter
    {
        public long? Checkpoint { get; set; }
        public Guid? AggregateId { get; set; }
        public string[] EventTypes { get; set; } = Array.Empty<string>();

        NonEmptyGuid? IEventFilter.AggregateId => AggregateId?.NonEmpty();

        ICollection<string> IEventFilter.EventTypes => EventTypes;
    }
}
