using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Common.EventStore.Lib.EfCore
{
    public class EventHostedService : IHostedService
    {
        private readonly IEventListener _eventListener;
        private CancellationTokenSource? _cancel;

        public EventHostedService(IEventListener eventListener)
        {
            _eventListener = eventListener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancel = new CancellationTokenSource();
            var _ = _eventListener.ListenAsync(_cancel.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancel?.Cancel();
            return Task.CompletedTask;
        }
    }
}
