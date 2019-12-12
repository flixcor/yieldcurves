using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Infrastructure;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace EventStore
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("request received");
            using var subscriber = new EventStoreSocketSubscriber("ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500");

            var cancel = context.CancellationToken;

            Task Action(Common.Core.IEvent e)
            {
                var type = e.GetType();
                _logger.LogInformation($"returning event: {type.Name}");
                
                var eventJson = JsonSerializer.Serialize(e, type);

                return responseStream.WriteAsync(new HelloReply
                {
                    Message = eventJson
                });
            }

            await subscriber.Subscribe(0, 0, Action, cancel);

            while (!cancel.IsCancellationRequested)
            {
                //Don't starve the request thread
                _logger.LogInformation("Not cancelled yet, going to sleep...");
                await Task.Delay(5000);
            }

            _logger.LogInformation("Cancelled");
        }
    }
}
