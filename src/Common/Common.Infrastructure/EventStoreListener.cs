using Common.Core;
using Common.Infrastructure.Extensions;
using EventStore.ClientAPI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class EventStoreListener : IMessageBusListener
    {
        private readonly IEventStoreConnection _connection;
        private readonly IEventBus _eventBus;
        private readonly IReadModelRepository<EventPosition> _currentPositionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _applicationName;

        public EventStoreListener(IEventStoreConnection connectionString, 
            IEventBus eventBus, 
            IReadModelRepository<EventPosition> currentPositionRepository, 
            ApplicationName applicationName,
            IUnitOfWork unitOfWork)
        {
            _connection = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _currentPositionRepository = currentPositionRepository ?? throw new ArgumentNullException(nameof(currentPositionRepository));
            _unitOfWork = unitOfWork;
            _applicationName = applicationName?.Value ?? throw new ArgumentNullException(nameof(applicationName));
        }

        public async Task SubscribeToAll()
        {
            var positionTask = GetCurrentPosition();
            var connectionTask = _connection.ConnectAsync();

            var position = await positionTask;
            await connectionTask;

            _connection.SubscribeToAllFrom(position.ToEventStorePosition(), CatchUpSubscriptionSettings.Default, PublishEvent);
        }

        private async Task PublishEvent(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            var @event = resolvedEvent.Deserialize();

            var currentEventPosition = await GetCurrentPosition();
            var currentPosition = currentEventPosition.ToEventStorePosition();
            var newPosition = resolvedEvent.OriginalPosition;

            if (currentPosition != null && newPosition != null && currentPosition > newPosition)
            {
                throw new Exception();
            }

            if (@event != null)
            {
                await _eventBus.Publish(@event);
            }

            var positionToInsert = newPosition.ToEventPosition(_applicationName);

            if (currentEventPosition == null)
            {
                positionToInsert.Id = Guid.NewGuid();
                await _currentPositionRepository.Insert(positionToInsert);
            }
            else
            {
                positionToInsert.Id = currentEventPosition.Id;
                await _currentPositionRepository.Update(positionToInsert);
            }

            if (_unitOfWork != null)
            {
                await _unitOfWork.SaveChanges();
            }
        }

        private Task<EventPosition> GetCurrentPosition()
        {
            return _currentPositionRepository.GetMany(x => x.ApplicationName == _applicationName).FirstOrDefaultAsync();
        }
    }
}
