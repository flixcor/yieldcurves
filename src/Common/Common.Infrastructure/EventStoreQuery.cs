using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Common.Core;
using Common.Infrastructure.Extensions;
using EventStore.ClientAPI;

namespace Common.Infrastructure
{
    public class EventStoreQuery : IDisposable
    {
        private readonly IEventStoreConnection _connection;
        private readonly HashSet<string> _eventTypes = new HashSet<string>();
        private const int PageSize = 250;

        public EventStoreQuery(string connectionString)
        {
            _connection = EventStoreConnection.Create(connectionString);
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async IAsyncEnumerable<(long, string, IEvent)> Run([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await _connection.ConnectAsync().ConfigureAwait(false);
            Position position = default;
            var isEndOfStream = false;

            while (!cancellationToken.IsCancellationRequested && !isEndOfStream)
            {
                var events = await _connection.ReadAllEventsForwardAsync(position, PageSize, false);

                isEndOfStream = events.IsEndOfStream;
                position = events.NextPosition;

                foreach ((var eventPosition, var name, var @event) in events.Events.Deserialize(_eventTypes.ToArray()))
                {
                    yield return (eventPosition.Value.CommitPosition, name, @event);
                }
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _connection.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
