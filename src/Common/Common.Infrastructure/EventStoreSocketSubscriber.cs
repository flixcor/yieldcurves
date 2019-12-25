using System;
using System.Collections.Generic;
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
        private long _position = 0;
        private Func<IEvent, string, long, Task> _action;
        private CancellationToken _cancellationToken;

        public EventStoreSocketSubscriber(string connectionString)
        {
            _connection = EventStoreConnection.Create(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async Task Subscribe(long commitPosition, Func<IEvent, string, long, Task> action, CancellationToken cancellationToken)
        {
            _position = commitPosition;
            _action = action;
            _cancellationToken = cancellationToken;

            var eventStorePosition = new Position(commitPosition, commitPosition);
            var typesArray = _eventTypes.ToArray();

            await _connection.ConnectAsync().ConfigureAwait(false);

            _connection.SubscribeToAllFrom(
                eventStorePosition,
                CatchUpSubscriptionSettings.Default, (sub, e) => PublishEvent(sub, e, action, cancellationToken),
                subscriptionDropped: OnDropped);
        }

        private Task PublishEvent(EventStoreCatchUpSubscription sub, ResolvedEvent resolvedEvent, Func<IEvent, string, long, Task> action, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var (position, type, data) = resolvedEvent.Deserialize(_eventTypes.ToArray());

                if (data != default)
                {
                    return action(data, type, position.CommitPosition);
                }
            }
            else
            {
                sub.Stop();
            }

            return Task.CompletedTask;
        }

        private void OnDropped(EventStoreCatchUpSubscription sub, SubscriptionDropReason reason, Exception exception)
        {
            if (reason != SubscriptionDropReason.UserInitiated)
            {
                Subscribe(_position, _action, _cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}
