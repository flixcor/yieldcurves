using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using EventStore.ClientAPI;

namespace Common.Infrastructure
{
    public class EventStoreSocketSubscriber : IDisposable
    {
        private readonly List<string> _eventTypes = new List<string>();
        private readonly IEventStoreConnection _connection;

        public EventStoreSocketSubscriber(string connectionString)
        {
            _connection = EventStoreConnection.Create(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async Task Subscribe(long preparePosition, long commitPosition, bool subscribe, Func<byte[], string, long, long, Task> action, CancellationToken cancellationToken)
        {
            var eventStorePosition = new Position(preparePosition, commitPosition);

            await _connection.ConnectAsync();

            var typesArray = _eventTypes.ToArray();
            var isEndOfStream = false;

            while (!isEndOfStream)
            {
                var slice = await _connection.ReadAllEventsForwardAsync(eventStorePosition, 200, false);
                eventStorePosition = slice.NextPosition;
                isEndOfStream = slice.IsEndOfStream;

                var events = slice.Events.ResolveEventBytes(typesArray);

                foreach ((var position, var type, var data) in events)
                {
                    await action(data, type, position.PreparePosition, position.CommitPosition);
                }
            }

            if (subscribe)
            {
                _connection.SubscribeToAllFrom(eventStorePosition, CatchUpSubscriptionSettings.Default, (_, e) => PublishEvent(e, action, cancellationToken));
            }
        }

        private Task PublishEvent(ResolvedEvent resolvedEvent, Func<byte[], string, long, long, Task> action, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var (type, data) = resolvedEvent.ResolveEventBytes(_eventTypes.ToArray());

                if (data != default)
                {
                    return action(data, type, resolvedEvent.OriginalPosition.Value.PreparePosition, resolvedEvent.OriginalPosition.Value.CommitPosition);
                }
            }

            return Task.CompletedTask;
        }
    }
}
