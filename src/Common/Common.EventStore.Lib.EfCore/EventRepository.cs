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

        public async Task Save(CancellationToken cancellationToken, params IEventWrapper[] events)
        {
            await _context.Events().AddRangeAsync(events.Select(PersistedEvent.FromEventWrapper));
            await _context.SaveChangesAsync(cancellationToken);
        }

        public IAsyncEnumerable<IEventWrapper> Get(CancellationToken cancellation)
        {
            return Get(EventFilter.None, cancellation);
        }

        public async IAsyncEnumerable<IEventWrapper> Get(IEventFilter eventFilter, [EnumeratorCancellation]CancellationToken cancellation)
        {
            var whereClause = eventFilter.ToWhereClause();

            await foreach (var item in _context
                .Events()
                .AsNoTracking()
                .Where(whereClause)
                .OrderBy(x => x.Id)
                .AsAsyncEnumerable()
                .WithCancellation(cancellation))
            {
                yield return item.ToWrapper();
            }
        }
    }
}
