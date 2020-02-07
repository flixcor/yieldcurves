using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    public class EventStoreQuery
    {
        private readonly HashSet<string> _eventTypes = new HashSet<string>();
        private readonly EventStoreClient _eventStoreClient;
        private const ulong PageSize = 250;

        public EventStoreQuery(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public void RegisterEventType(string eventType) => _eventTypes.Add(eventType);

        public async IAsyncEnumerable<IEventWrapper> Run([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var filter = _eventTypes.Any()
                ? new EventTypeFilter(_eventTypes.Select(x => new PrefixFilterExpression(x)).ToArray())
                : EventTypeFilter.None;

            var events = _eventStoreClient.ReadAllAsync(Direction.Forwards, Position.Start, PageSize, false, filter, cancellationToken: cancellationToken);

            await foreach (var item in events.WithCancellation(cancellationToken))
            {
                var wrapper = item.Deserialize();
                if (wrapper != null)
                {
                    yield return wrapper.Value.Item1;
                }
            }
        }
    }
}
