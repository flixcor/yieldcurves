using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Common.EventStore.Lib
{
    public class EventListener : IMessageBusListener
    {
        private readonly IEventReadRepository _eventReadRepository;
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _applicationName;

        public EventListener(IEventReadRepository eventReadRepository,
            IEventBus eventBus,
            ApplicationName applicationName,
            IServiceProvider serviceProvider)
        {
            _eventReadRepository = eventReadRepository;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _serviceProvider = serviceProvider;
            _applicationName = applicationName?.Value ?? throw new ArgumentNullException(nameof(applicationName));
        }

        public async Task SubscribeToAll(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var unitOfWork = services.GetService<IUnitOfWork>();
            var repo = services.GetRequiredService<IReadModelRepository<EventPosition>>();

            var currentPosition = await GetCurrentPosition(repo);

            var filter = EventFilter.FromCheckpoint(currentPosition.CommitPosition);

            await foreach (var (wrapper, _) in _eventReadRepository.Subscribe(filter).WithCancellation(cancellationToken))
            {
                var newPosition = new EventPosition(wrapper.Id, wrapper.Id, _applicationName)
                {
                    Id = currentPosition.Id
                };

                await _eventBus.Publish(wrapper, cancellationToken);

                await repo.Update(newPosition);

                if (unitOfWork != null)
                {
                    await unitOfWork.SaveChanges();
                }
            }
        }

        private async Task<EventPosition> GetCurrentPosition(IReadModelRepository<EventPosition> repo)
        {
            var existing = await repo
                .Single(x => x.ApplicationName == _applicationName);

            if (existing == null)
            {
                existing = new EventPosition(0, 0, _applicationName);
                await repo.Insert(existing);
            }

            return existing;
        }
    }
}
