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
                var prefixes = string.Join("|", filter.EventTypes);
                return new EventTypeFilter(new RegularFilterExpression(prefixes));
            }

            return new EventTypeFilter(RegularFilterExpression.ExcludeSystemEvents);
        }
    }
}
