using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
            }
        }

        private async Task SendToAll(long id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var result = await conn.GetAsync(id);

            EventChannel.Publish(result);

            await conn.CloseAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
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
