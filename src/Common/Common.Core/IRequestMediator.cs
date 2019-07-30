using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IRequestMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> query, CancellationToken cancellationToken = default);

        Task<Result> Send(IRequest command, CancellationToken cancellationToken = default);
    }
}
