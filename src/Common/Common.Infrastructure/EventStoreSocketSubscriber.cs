using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.SignalR;
using EventStore.ClientAPI;

namespace Common.Infrastructure
{
    public class EventStoreSocketSubscriber
    {
        private readonly IEventStoreConnection _connection;
        private readonly ISocketContext _socketContext;

        private readonly List<string> _eventTypes = new List<string>();

        public EventStoreSocketSubscriber(IEventStoreConnection connection, ISocketContext socketContext)
        {
            _connection = connection;
            _socketContext = socketContext;
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async Task Subscribe(string userId, long preparePosition, long commitPosition, CancellationToken cancellationToken)
        {
            var eventStorePosition = new Position(preparePosition, commitPosition);

            await _connection.ConnectAsync();
            _connection.SubscribeToAllFrom(eventStorePosition, CatchUpSubscriptionSettings.Default, (_, e) => PublishEvent(e, userId, preparePosition, commitPosition, cancellationToken));
        }

        private Task PublishEvent(ResolvedEvent resolvedEvent, string userId, long preparePosition, long commitPosition, CancellationToken cancellationToken)
        {
            var @event = resolvedEvent.Deserialize();

            return !_eventTypes.Any() || _eventTypes.Contains(@event.GetType().Name)
                ? _socketContext.SendToUser(@event, userId, preparePosition, commitPosition, cancellationToken)
                : Task.CompletedTask;
        }
    }
}
