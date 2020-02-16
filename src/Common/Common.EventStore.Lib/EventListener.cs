using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib
{
    public class EventStoreListener : IMessageBusListener
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly IEventBus _eventBus;
        private readonly IReadModelRepository<EventPosition> _currentPositionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _applicationName;

        public EventStoreListener(IEventReadRepository eventReadRepository,
            IEventBus eventBus,
            IReadModelRepository<EventPosition> currentPositionRepository,
            ApplicationName applicationName,
            IUnitOfWork unitOfWork)
        {
            _eventReadRepository = eventReadRepository;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _currentPositionRepository = currentPositionRepository ?? throw new ArgumentNullException(nameof(currentPositionRepository));
            _unitOfWork = unitOfWork;
            _applicationName = applicationName?.Value ?? throw new ArgumentNullException(nameof(applicationName));
        }

        public async Task SubscribeToAll(CancellationToken cancellationToken)
        {
            var currentPosition = await GetCurrentPosition();

            var filter = EventFilter.FromCheckpoint(currentPosition.CommitPosition);

            await foreach (var (wrapper, _) in _eventReadRepository.Subscribe(filter).WithCancellation(cancellationToken))
            {
                var newPosition = new EventPosition(wrapper.Id, wrapper.Id, _applicationName)
                {
                    Id = currentPosition.Id
                };

                await _eventBus.Publish(wrapper, cancellationToken);

                await _currentPositionRepository.Update(newPosition);

                if (_unitOfWork != null)
                {
                    await _unitOfWork.SaveChanges();
                }
            }
        }

        private async Task<EventPosition> GetCurrentPosition()
        {
            var existing = await _currentPositionRepository
                .Single(x => x.ApplicationName == _applicationName);

            if (existing == null)
            {
                existing = new EventPosition(0, 0, _applicationName);
                await _currentPositionRepository.Insert(existing);
            }

            return existing;
        }
    }
}
