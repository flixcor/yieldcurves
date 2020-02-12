using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Text;
using Npgsql;

namespace Common.EventStore.Lib.Postgres
{
    public interface IEventListener
    {
        Task ListenAsync(CancellationToken cancellationToken);
    }

    internal class EventListener : IEventListener, IHostedService
    {
        private readonly CancellationTokenSource _source;
        private readonly CancellationToken _token;
        private readonly string _connectionString;

        public EventListener(string connectionString)
        {
            _source = new CancellationTokenSource();
            _token = _source.Token;
            _connectionString = connectionString;
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);


            await connection.OpenAsync();
            connection.Notification += SendToAll;

            using (var cmd = new NpgsqlCommand("LISTEN new_event", connection))
            {
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            try
            {
                while (!_token.IsCancellationRequested)
                {
                    await connection.WaitAsync(cancellationToken);   // Thread will block here
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                connection.Notification -= SendToAll;
                await connection.CloseAsync();
            }
        }

        private void SendToAll(object obj, NpgsqlNotificationEventArgs e)
        {
            if (long.TryParse(e.Payload, out var id))
            {
                var _ = SendToAll(id);
                return;
            }

            var doc = JsonDocument.Parse(e.Payload);
            SendToAll(doc);
        }

        private async Task SendToAll(long id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var result = await conn.GetAsync(id);

            EventChannel.Publish(result);

            await conn.CloseAsync();
        }

        private void SendToAll(JsonDocument jsonDocument)
        {
            var aggregateId = jsonDocument.RootElement.GetProperty("AggregateId").GetGuid();
            var eventType = jsonDocument.RootElement.GetProperty("EventType").GetString();
            var id = jsonDocument.RootElement.GetProperty("Id").GetInt64();
            var timeStamp = InstantPattern.General.Parse(jsonDocument.RootElement.GetProperty("Timestamp").GetString()).Value;
            var version = jsonDocument.RootElement.GetProperty("Version").GetInt32();
            var payload = jsonDocument.RootElement.GetProperty("Payload").GetBytesFromBase64();
            var metadata = jsonDocument.RootElement.GetProperty("Metadata").GetBytesFromBase64();

            var result = new PersistedEvent
            {
                AggregateId = aggregateId,
                EventType = eventType,
                Id = id,
                Metadata = metadata,
                Payload = payload,
                Timestamp = timeStamp,
                Version = version
            };

            EventChannel.Publish(result);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
            var _ = ListenAsync(_token);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _source.Cancel();
            await Task.Delay(500);
        }
    }
}
