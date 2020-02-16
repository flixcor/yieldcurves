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
            var eventType = wrapper.GetContent().GetType();

            var handlerTypes = eventType
                .GetInterfaces()
                .Where(i => i.GetInterface(nameof(IEvent)) != null)
                .Select(i => typeof(IHandleEvent<>).MakeGenericType(i));

            var handlers = handlerTypes
                .SelectMany(t => _serviceProvider.GetServices(t))
                .Cast<dynamic>();

            var concrete = wrapper.ConcreteGeneric();

            foreach (var handler in handlers)
            {
                await handler.Handle(concrete, cancellationToken);
            }
        }
    }
}
