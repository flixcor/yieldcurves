using System;

namespace Common.Infrastructure
{
    public class EventHeaders
    {
        public string EventClrTypeName { get; internal set; }
        public string AggregateClrTypeName { get; internal set; }
        public Guid CommitId { get; internal set; }
    }
}
