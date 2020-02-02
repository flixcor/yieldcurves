using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IHandleCommand<in TCommand>
        where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface IHandleEvent<TEvent>
        where TEvent : IEvent
    {
        Task Handle(IEventWrapper<TEvent> @event, CancellationToken cancellationToken);
    }

    public interface IHandleQuery<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }

    public interface IHandleListQuery<TQuery, out TResponse>
        where TQuery : IListQuery<TResponse>
    {
        IAsyncEnumerable<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
