using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql;

namespace Common.EventStore.Lib.EfCore
{
    public interface IEventListener
    {
        Task ListenAsync(CancellationToken cancellationToken);
    }

    public class EventListener : IEventListener
    {
        private readonly NpgsqlConnection _connection;

        public EventListener(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            await _connection.OpenAsync();
            _connection.TypeMapper.UseNodaTime();
            _connection.Notification += SendToAll;

            using (var cmd = new NpgsqlCommand("LISTEN new_event", _connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _connection.WaitAsync(cancellationToken);   // Thread will block here
                }
            }
            catch (OperationCanceledException)
            {
            }

            _connection.Notification -= SendToAll;
            await _connection.CloseAsync();
        }

        private void SendToAll(object obj, NpgsqlNotificationEventArgs e)
        {
            if (long.TryParse(e.Payload, out var id))
            {
                SendToAll(id);
            }

        }

        private static readonly string[] s_columnNames = { "Timestamp", "AggregateId", "Version", "EventType", "Metadata", "Payload" };
        private static readonly string s_queryPart = string.Join(", ", s_columnNames.Select(x => $"\"{x}\"").ToArray());
        private static readonly string s_query = $"SELECT {s_queryPart} from public.\"Events\"";

        private void SendToAll(long id)
        {
            var query = $"{s_query} where \"Id\" = @id";

            using var conn = new NpgsqlConnection(_connection.ConnectionString);
            conn.Open();
            conn.TypeMapper.UseNodaTime();

            using var command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("id", id);
            using var ding = command.ExecuteReader();

            ding.Read();

            var timestamp = ding.GetFieldValue<Instant>(0);
            var aggregateId = ding.GetFieldValue<Guid>(1);
            var version = ding.GetFieldValue<int>(2);
            var eventType = ding.GetFieldValue<string>(3);
            var metadata = ding.GetFieldValue<byte[]>(4);
            var payload = ding.GetFieldValue<byte[]>(5);

            var result = new PersistedEvent()
            {
                AggregateId = aggregateId,
                EventType = eventType,
                Id = id,
                Metadata = metadata,
                Payload = payload,
                Timestamp = timestamp,
                Version = version
            };
            EventChannel.Publish(result);
        }
    }
}
