using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.EventStore.Lib;
using Common.EventStore.Lib.Postgres;
using NodaTime;

namespace Npgsql
{
    internal static class ConnectionExtensions
    {
        private static readonly string[] s_columnNames = { "Timestamp", "AggregateId", "Version", "EventType", "Metadata", "Payload" };
        private static readonly string s_queryColumns = string.Join(", ", s_columnNames.Select(x => $"\"{x}\"").ToArray());
        private const string Table = "public.\"Events\"";
        private const string Select = "SELECT";
        private const string From = "from";

        public static async Task<PersistedEvent> GetAsync(this NpgsqlConnection conn, long eventId, CancellationToken cancellationToken = default)
        {
            var query = $"{Select} {s_queryColumns} {From} {Table} where \"Id\" = @id";

            await using var command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("id", eventId);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            await reader.ReadAsync(cancellationToken);

            var timestamp = reader.GetFieldValue<Instant>(0);
            var aggregateId = reader.GetFieldValue<Guid>(1);
            var version = reader.GetFieldValue<int>(2);
            var eventType = reader.GetFieldValue<string>(3);
            var metadata = reader.GetFieldValue<byte[]>(4);
            var payload = reader.GetFieldValue<byte[]>(5);

            var result = new PersistedEvent()
            {
                AggregateId = aggregateId,
                EventType = eventType,
                Id = eventId,
                Metadata = metadata,
                Payload = payload,
                Timestamp = timestamp,
                Version = version
            };

            return result;
        }

        public static async IAsyncEnumerable<PersistedEvent> GetAsync(this NpgsqlConnection conn, IEventFilter eventFilter, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var parameters = new List<(string, object)>();
            var whereClauses = new List<string>();

            if (eventFilter.AggregateId.HasValue)
            {
                var name = "a";
                whereClauses.Add($"\"AggregateId\" = @{name}");
                parameters.Add((name, eventFilter.AggregateId.Value));
            }

            if (eventFilter.Checkpoint.HasValue)
            {
                var name = "b";
                whereClauses.Add($"\"Id\" > @{name}");
                parameters.Add((name, eventFilter.Checkpoint.Value));
            }

            var eventTypes = eventFilter.EventTypes.ToArray();

            if (eventTypes.Any())
            {
                var stringBuilder = new StringBuilder("\"EventType\" in (");
                var whereClauseParts = new string[eventTypes.Length];

                for (var i = 0; i < eventTypes.Length; i++)
                {
                    var eventType = eventTypes[i];
                    var name = "c" + i;
                    parameters.Add((name, eventType));
                    whereClauseParts[i] = "@" + name;
                }

                var partsString = string.Join(", ", whereClauseParts);

                stringBuilder.Append(partsString);
                stringBuilder.Append(")");

                whereClauses.Add(stringBuilder.ToString());
            }

            var whereClause = whereClauses.Any()
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : "";

            const string Id = "\"Id\"";

            var query = $"{Select} {s_queryColumns}, {Id} {From} {Table} {whereClause} order by {Id}";

            await using var command = new NpgsqlCommand(query, conn);

            foreach (var (parameterName, value) in parameters)
            {
                command.Parameters.AddWithValue(parameterName, value);
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var timestamp = reader.GetFieldValue<Instant>(0);
                var aggregateId = reader.GetFieldValue<Guid>(1);
                var version = reader.GetFieldValue<int>(2);
                var eventType = reader.GetFieldValue<string>(3);
                var metadata = reader.GetFieldValue<byte[]>(4);
                var payload = reader.GetFieldValue<byte[]>(5);
                var eventId = reader.GetFieldValue<long>(6);

                var result = new PersistedEvent()
                {
                    AggregateId = aggregateId,
                    EventType = eventType,
                    Id = eventId,
                    Metadata = metadata,
                    Payload = payload,
                    Timestamp = timestamp,
                    Version = version
                };

                yield return result;
            }
        }
    }
}
