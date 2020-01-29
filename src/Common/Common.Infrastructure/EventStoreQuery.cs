using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Common.Core;
using Common.Infrastructure.Extensions;
using EventStore.Client;

namespace Common.Infrastructure
{
    public class EventStoreQuery
    {
        private readonly HashSet<string> _eventTypes = new HashSet<string>();
        private readonly EventStoreClient _client;

        public EventStoreQuery(EventStoreClient client)
        {
            _client = client;
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async IAsyncEnumerable<(ulong, string, IEvent)> Run([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            Position position = default;

            var events = _client.ReadAllAsync(Direction.Forwards, position, ulong.MaxValue, userCredentials: new UserCredentials("admin", "changeit"));

            await foreach (var e in events)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                (var eventPosition, var name, var @event) = e.Deserialize(_eventTypes.ToArray());
                yield return (eventPosition.Value.CommitPosition, name, @event);
            }
        }
    }
}
