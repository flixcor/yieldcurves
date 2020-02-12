using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Npgsql;

namespace Common.EventStore.Lib.Postgres
{
    internal class EventRepository : IEventWriteRepository, IEventReadRepository
    {
        private readonly string _connectionString;

        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Save(CancellationToken cancellationToken = default, params (IEventWrapper, IMetadata)[] events)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await using var trans = await connection.BeginTransactionAsync(cancellationToken);

                foreach (var tup in events)
                {
                    var persisted = PersistedEvent.FromEventWrapper(tup);

                    await using var cmd = new NpgsqlCommand(@" INSERT INTO public.""Events"" (""Timestamp"", ""AggregateId"", ""Version"", ""EventType"", ""Metadata"", ""Payload"") VALUES (@t, @a, @v, @et, @m, @p)", connection);
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
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async IAsyncEnumerable<(IEventWrapper, IMetadata)> Get(IEventFilter? eventFilter = null, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            eventFilter ??= EventFilter.None;

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            try
            {
                await foreach (var item in connection.GetAsync(eventFilter, cancellationToken).WithCancellation(cancellationToken))
                {
                    yield return item.ToWrapper();
                }
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public IAsyncEnumerable<(IEventWrapper, IMetadata)> Subscribe(IEventFilter? eventFilter = null, CancellationToken cancellation = default)
        {
            eventFilter ??= EventFilter.None;
            return EventChannel.Subscribe(cancellation).Select(x => x.ToWrapper());
        }
    }
}
