using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.EntityFrameworkCore;

namespace Common.EventStore.Lib.EfCore
{
    internal class EventRepository : IEventWriteRepository, IEventReadRepository
    {
        private readonly EventStoreContext _context;

        public EventRepository(EventStoreContext context)
        {
            _context = context;
        }

        public async Task Save(CancellationToken cancellationToken = default, params (IEventWrapper, IMetadata)[] events)
        {
            await _context.Events().AddRangeAsync(events.Select(PersistedEvent.FromEventWrapper));
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async IAsyncEnumerable<(IEventWrapper, IMetadata)> Get(IEventFilter? eventFilter = null, [EnumeratorCancellation]CancellationToken cancellation = default)
        {
            eventFilter ??= EventFilter.None;

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
