using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Common.EventStore.Lib.EfCore
{
    internal class EventRepository : IEventWriteRepository, IEventReadRepository
    {
        private readonly EventStoreContext _context;
        private readonly string _connectionString;

        public EventRepository(EventStoreContext context, string connectionString)
        {
            _context = context;
            _connectionString = connectionString;
        }

        public async Task Save(CancellationToken cancellationToken = default, params (IEventWrapper, IMetadata)[] events)
        {
            //await _context.Events().AddRangeAsync(events.Select(PersistedEvent.FromEventWrapper));
            //await _context.SaveChangesAsync(cancellationToken);

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            conn.TypeMapper.UseNodaTime();
            await using var trans = await conn.BeginTransactionAsync(cancellationToken);

            foreach (var tup in events)
            {
                var persisted = PersistedEvent.FromEventWrapper(tup);

                await using var cmd = new NpgsqlCommand(@" INSERT INTO public.""Events"" (""Timestamp"", ""AggregateId"", ""Version"", ""EventType"", ""Metadata"", ""Payload"") VALUES (@t, @a, @v, @et, @m, @p)", conn);
                cmd.Parameters.AddWithValue("t", persisted.Timestamp);
                cmd.Parameters.AddWithValue("a", persisted.AggregateId);
                cmd.Parameters.AddWithValue("v", persisted.Version);
                cmd.Parameters.AddWithValue("et", persisted.EventType);
                cmd.Parameters.AddWithValue("m", persisted.Metadata);
                cmd.Parameters.AddWithValue("p", persisted.Payload);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            await trans.CommitAsync(cancellationToken);
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

        public IAsyncEnumerable<(IEventWrapper, IMetadata)> Subscribe(IEventFilter? eventFilter = null, CancellationToken cancellation = default)
        {
            eventFilter ??= EventFilter.None;
            return EventChannel.Subscribe(cancellation).Select(x => x.ToWrapper());
        }
    }
}
