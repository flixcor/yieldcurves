using Common.Core;
using EventStore.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.EventStore.Lib.GES
{
    public class EventStoreListener : IMessageBusListener
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly IEventBus _eventBus;
        private readonly IReadModelRepository<EventPosition> _currentPositionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _applicationName;

        public EventStoreListener(EventStoreClient eventStoreClient,
            IEventBus eventBus,
            IReadModelRepository<EventPosition> currentPositionRepository,
            ApplicationName applicationName,
            IUnitOfWork unitOfWork)
        {
            _eventStoreClient = eventStoreClient;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _currentPositionRepository = currentPositionRepository ?? throw new ArgumentNullException(nameof(currentPositionRepository));
            _unitOfWork = unitOfWork;
            _applicationName = applicationName?.Value ?? throw new ArgumentNullException(nameof(applicationName));
        }

        public async Task SubscribeToAll()
        {
            var positionTask = GetCurrentPosition();

            var position = await positionTask;

            _eventStoreClient.SubscribeToAll(start: position.ToEventStorePosition(), PublishEvent);
        }

        private async Task PublishEvent(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            var tup = resolvedEvent.Deserialize();

            if (tup == null)
            {
                return;
            }

            var (wrapper, _) = tup.Value;

            var currentEventPosition = await GetCurrentPosition();
            var currentPosition = currentEventPosition.ToEventStorePosition();
            var newPosition = resolvedEvent.OriginalPosition ?? Position.Start;

            if (currentPosition != null && newPosition != null && currentPosition > newPosition)
            {
                throw new Exception();
            }

            if (wrapper != null)
            {
                await _eventBus.Publish(wrapper);
            }

            var positionToInsert = newPosition.ToEventPosition(_applicationName);

            if (currentEventPosition == null)
            {
                positionToInsert.Id = NonEmptyGuid.New();
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

        private async Task<EventPosition> GetCurrentPosition()
        {
            return await _currentPositionRepository
                .GetMany(x => x.ApplicationName == _applicationName)
                .FirstOrDefaultAsync() ?? new EventPosition(0, 0, _applicationName);
        }
    }
}
