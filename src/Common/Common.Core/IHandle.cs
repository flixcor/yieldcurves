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

    public interface IHandleEvent<in TEvent>
        where TEvent : IEvent
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }

    public interface IHandleQuery<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }

    public interface IHandleQueryMaybe<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
        Task<Maybe<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
    }

    public interface IHandleListQuery<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        IAsyncEnumerable<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
