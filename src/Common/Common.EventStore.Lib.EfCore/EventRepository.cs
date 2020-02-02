using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.EntityFrameworkCore;

namespace Common.EventStore.Lib.EfCore
{
    internal class EventRepository : IEventRepository
    {
        private readonly EventStoreContext _context;

        public EventRepository(EventStoreContext context)
        {
            _context = context;
        }

        public async Task SaveEvents(CancellationToken cancellationToken, params IEventWrapper[] events)
        {
            await _context.Events().AddRangeAsync(events.Select(PersistedEvent.FromEventWrapper));
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async IAsyncEnumerable<IEventWrapper> GetEvents(Guid aggregateId, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            await foreach (var item in _context
                .Events()
                .AsNoTracking()
                .Where(x => x.AggregateId == aggregateId)
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item.ToWrapper();
            }
        }
    }
}
