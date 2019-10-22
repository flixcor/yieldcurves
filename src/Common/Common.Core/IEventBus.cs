using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IEventBus
    {
        Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
    }
}
