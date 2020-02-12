using System;
using System.Linq;
using System.Linq.Expressions;
using Common.EventStore.Lib.Postgres;

namespace Common.EventStore.Lib
{
    internal static class EventFilterExtensions
    {
        public static Expression<Func<PersistedEvent, bool>> ToWhereClause(this IEventFilter eventFilter) => e =>
            (!eventFilter.AggregateId.HasValue || eventFilter.AggregateId == e.AggregateId) &&
            (!eventFilter.Checkpoint.HasValue || eventFilter.Checkpoint > e.Id) &&
            (!eventFilter.EventTypes.Any() || eventFilter.EventTypes.Contains(e.EventType));
    }
}
