using System.Threading;
using System.Threading.Tasks;

namespace Common.Core
{
    public interface IMessageBusListener
    {
        Task SubscribeToAll(CancellationToken cancellationToken = default);
    }
}
