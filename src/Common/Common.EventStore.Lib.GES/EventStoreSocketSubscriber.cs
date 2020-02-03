using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    public class EventStoreSocketSubscriber
    {
        private readonly List<string> _eventTypes = new List<string>();
        private readonly EventStoreClient _eventStoreClient;
        private long _position = 0;
        private Func<IEventWrapper, Task> _action = w => Task.CompletedTask;

        public EventStoreSocketSubscriber(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public Task Subscribe(long commitPosition, Func<IEventWrapper, Task> action, CancellationToken cancellationToken)
        {
            _position = commitPosition;
            _action = action;

            var eventStorePosition = new Position((ulong)commitPosition, (ulong)commitPosition);
            var typesArray = _eventTypes.ToArray();

            var eventstorePosition = Position.FromInt64(_position, _position);

            _eventStoreClient.SubscribeToAll(eventstorePosition, Pub, cancellationToken: cancellationToken);

            return Task.CompletedTask;
        }

        private async Task Pub(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            _position = (long?)resolvedEvent.OriginalPosition?.CommitPosition ?? _position;

            var wrapper = resolvedEvent.Deserialize(_eventTypes.ToArray());

            if (wrapper != null && !cancellationToken.IsCancellationRequested)
            {
                await _action(wrapper);
            }
        }
    }
}
