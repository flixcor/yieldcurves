using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using EventStore.Client;

namespace Common.Infrastructure
{
    public class EventStoreSocketSubscriber
    {
        private readonly List<string> _eventTypes = new List<string>();
        private readonly EventStoreClient _client;
        private ulong _position = 0;
        private Func<IEvent, string, ulong, Task> _action;
        private CancellationToken _cancellationToken;

        public EventStoreSocketSubscriber(EventStoreClient client)
        {
            _client = client;
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public Task Subscribe(ulong commitPosition, Func<IEvent, string, ulong, Task> action, CancellationToken cancellationToken)
        {
            _position = commitPosition;
            _action = action;
            _cancellationToken = cancellationToken;

            var eventStorePosition = new Position(commitPosition, commitPosition);
            var typesArray = _eventTypes.ToArray();

            _client.SubscribeToAll(
                start: eventStorePosition,
                eventAppeared: (sub, e, c) => PublishEvent(sub, e, action, c),
                subscriptionDropped: OnDropped,
                userCredentials: new UserCredentials("admin", "changeit"));

            return Task.CompletedTask;
        }

        private Task PublishEvent(StreamSubscription sub, ResolvedEvent resolvedEvent, Func<IEvent, string, ulong, Task> action, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var (position, type, data) = resolvedEvent.Deserialize(_eventTypes.ToArray());

                if (data != default)
                {
                    return action(data, type, position?.CommitPosition ?? 0);
                }
            }
            else
            {
                sub.Dispose();
            }

            return Task.CompletedTask;
        }

        private void OnDropped(StreamSubscription sub, SubscriptionDroppedReason reason, Exception exception)
        {
            if (reason != SubscriptionDroppedReason.Disposed)
            {
                Subscribe(_position, _action, _cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}
