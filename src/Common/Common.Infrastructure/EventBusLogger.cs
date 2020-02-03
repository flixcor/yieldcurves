using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.Infrastructure
{
    public class EventBusLogger : IEventBus
    {
        private readonly IEventBus _eventBus;

        public EventBusLogger(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task Publish(IEventWrapper @event, CancellationToken cancellationToken = default)
        {
            var eventName = @event.Content.GetType().Name;
            var aggregateId = @event.Metadata.AggregateId;
            var version = @event.Metadata.Version;

            Console.WriteLine($"processing event of type {eventName} with id {aggregateId} and version {version}");
            try
            {
                await _eventBus.Publish(@event, cancellationToken);
                Console.WriteLine($"successfully processed event of type {eventName} with id {aggregateId} and version {version}");
            }
            catch (Exception)
            {
                Console.WriteLine($"something went wrong while processing event of type {eventName} with id {aggregateId} and version {version}");
                throw;
            }
        }
    }
}
