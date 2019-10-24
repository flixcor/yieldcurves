using System;
using System.Collections.Generic;
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
            var handlerType = typeof(IHandleEvent<>)
                .MakeGenericType(@event.GetType());

            IEnumerable<dynamic> handlers = _serviceProvider.GetServices(handlerType) ?? new List<dynamic>();

            foreach (var handler in handlers)
            {
                await handler.Handle((dynamic)@event, cancellationToken);
            }
        }
    }
}
