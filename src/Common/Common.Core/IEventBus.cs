using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IEventBus
    {
        Task Publish(IEventWrapper @event, CancellationToken cancellationToken = default);
    }
}
