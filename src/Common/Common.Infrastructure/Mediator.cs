using Common.Core;
using Common.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class Mediator : IRequestMediator, IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : Event
        {
            var handlerType = typeof(IHandleEvent<>)
                .MakeGenericType(@event.GetType());

            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            IEnumerable<dynamic> handlers = services.GetServices(handlerType) ?? new List<dynamic>();

            var tasks = handlers.Select(handler => handler.Handle((dynamic)@event, cancellationToken)).Cast<Task>();

            await Task.WhenAll(tasks);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> query, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IHandleQuery<,>)
                .MakeGenericType(query.GetType(), typeof(TResponse));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType) ?? throw new HandlerNotFoundException(handlerType);

            return handler.Handle((dynamic)query, cancellationToken);
        }

        public Task<Result> Send(IRequest command, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IHandleCommand<>)
                .MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetRequiredService(handlerType) ?? throw new HandlerNotFoundException(handlerType);

            return handler.Handle((dynamic)command, cancellationToken);
        }
    }
}
