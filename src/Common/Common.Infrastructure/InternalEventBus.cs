using System;
using System.Collections.Generic;
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

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
        {
            var handlerTypes = @event.GetType()
                .GetInterfaces()
                .Where(i => i.GetInterface(nameof(IEvent)) != null)
                .Select(i => typeof(IHandleEvent<>).MakeGenericType(i));

            var handlers = handlerTypes
                .SelectMany(t => _serviceProvider.GetServices(t))
                .Cast<dynamic>();

            foreach (var handler in handlers)
            {
                await handler.Handle((dynamic)@event, cancellationToken);
            }
        }
    }
}
