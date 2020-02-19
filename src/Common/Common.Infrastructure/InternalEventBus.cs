using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure
{
    public class InternalEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InternalEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task Publish(IEventWrapper wrapper, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();

            var eventType = wrapper.GetContent().GetType();

            var eventInterfaceTypes = eventType
                .GetInterfaces()
                .Where(i => i.GetInterface(nameof(IEvent)) != null);

            foreach (var eventInterfaceType in eventInterfaceTypes)
            {
                var handlerType = typeof(IHandleEvent<>).MakeGenericType(eventInterfaceType);
                var concrete = wrapper.ConcreteGeneric(eventInterfaceType);

                var handlers = scope.ServiceProvider.GetServices(handlerType).Cast<dynamic>();

                foreach (var handler in handlers)
                {
                    await handler.Handle(concrete, cancellationToken);
                }
            }
        }
    }
}
