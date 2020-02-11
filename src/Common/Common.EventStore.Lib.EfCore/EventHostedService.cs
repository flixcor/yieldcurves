using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Common.EventStore.Lib.EfCore
{
    public class EventHostedService : IHostedService, IAsyncDisposable, IDisposable
    {
        private readonly NpgsqlConnection _connection;
        private readonly IEventListener _eventListener;

        public EventHostedService(NpgsqlConnection connection, IEventListener eventListener)
        {
            _connection = connection;
            _eventListener = eventListener;
        }

        public void Dispose() => _connection?.Dispose();

        public ValueTask DisposeAsync() => _connection is null ? new ValueTask() : _connection.DisposeAsync();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _connection.OpenAsync();
            var _ = _eventListener.ListenAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _connection is null ? Task.CompletedTask : _connection.CloseAsync();
        }
    }
}
