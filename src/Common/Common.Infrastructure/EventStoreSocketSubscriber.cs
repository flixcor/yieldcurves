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

        public async Task Subscribe(long preparePosition, long commitPosition, Func<IEvent, Task> action, CancellationToken cancellationToken)
        {
            var eventStorePosition = new Position(preparePosition, commitPosition);

            await _connection.ConnectAsync();
            _connection.SubscribeToAllFrom(eventStorePosition, CatchUpSubscriptionSettings.Default, (_, e) => PublishEvent(e, action, cancellationToken));
        }

        private Task PublishEvent(ResolvedEvent resolvedEvent, Func<IEvent, Task> action, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var @event = resolvedEvent.Deserialize();

                if (@event != null && (!_eventTypes.Any() || _eventTypes.Contains(@event.GetType().Name)))
                {
                    return action(@event);
                }
            }

            return Task.CompletedTask;
        }
    }
}
