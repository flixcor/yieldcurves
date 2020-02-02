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

            Console.WriteLine($"processing event of type {eventName} with id {@event.AggregateId} and version {@event.Version}");
            try
            {
                await _eventBus.Publish(@event, cancellationToken);
                Console.WriteLine($"successfully processed event of type {eventName} with id {@event.AggregateId} and version {@event.Version}");
            }
            catch (Exception)
            {
                Console.WriteLine($"something went wrong while processing event of type {eventName} with id {@event.AggregateId} and version {@event.Version}");
                throw;
            }
        }
    }
}
