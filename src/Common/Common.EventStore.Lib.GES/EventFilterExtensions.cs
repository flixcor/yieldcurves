using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    public static class EventFilterExtensions
    {
        public static EventTypeFilter ToGESFilter(this IEventFilter filter)
        {
            if (filter.EventTypes.Any())
            {
                var prefixes = filter.EventTypes.Select(x => new PrefixFilterExpression(x)).ToArray();
                var eventTypeFilter = new EventTypeFilter(prefixes);
            }

            return EventTypeFilter.None;
        }
    }
}
