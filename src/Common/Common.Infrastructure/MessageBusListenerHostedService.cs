using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure
{
    public class MessageBusListenerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _source = new CancellationTokenSource();
        private readonly IMessageBusListener _messageBusListener;

        public MessageBusListenerHostedService(IMessageBusListener messageBusListener)
        {
            _messageBusListener = messageBusListener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var _ = _messageBusListener.SubscribeToAll(_source.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _source.Cancel();
            return Task.CompletedTask;
        }
    }
}
