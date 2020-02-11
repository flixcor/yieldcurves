using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
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
            async void Handler(object o, NpgsqlNotificationEventArgs e) => await SendToAllAsync(o, cancellationToken);


            _connection.Notification += Handler;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _connection.WaitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _connection.Notification -= Handler;
            }
        }

        private Task SendToAllAsync(object obj, CancellationToken cancellationToken)
        {
            if (obj is ValueTuple<IEventWrapper, IMetadata> tup)
            {
                return EventChannel.PublishAsync(tup, cancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}
