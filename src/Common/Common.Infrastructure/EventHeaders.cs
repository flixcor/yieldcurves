using System;

namespace Common.Infrastructure
{
    public class EventHeaders
    {
        public string EventName { get; internal set; }
        public string AggregateName { get; internal set; }
        public Guid CommitId { get; internal set; }
    }
}
