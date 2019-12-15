using System.Threading.Tasks;
using Common.Infrastructure;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace EventStore
{
    public class EventStoreService : EventStore.EventStoreBase
    {
        private readonly ILogger<EventStoreService> _logger;
        public EventStoreService(ILogger<EventStoreService> logger)
        {
            _logger = logger;
        }

        public override async Task GetEvents(EventRequest request, IServerStreamWriter<EventReply> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("request received");
            using var subscriber = new EventStoreSocketSubscriber("ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500");

            foreach (var type in request.EventTypes)
            {
                subscriber.RegisterEventType(type);
            }

            var cancel = context.CancellationToken;

            Task Action(byte[] data, string type, long preparePosition, long commitPosition)
            {
                var reply = new EventReply
                {
                    PreparePosition = preparePosition,
                    CommitPosition = commitPosition,
                    EventType = type,
                    Payload = ByteString.CopyFrom(data)
                };

                _logger.LogInformation($"returning event:");

                return responseStream.WriteAsync(reply);
            }

            await subscriber.Subscribe(request.PreparePosition, request.CommitPosition, request.Subscribe, Action, cancel);

            while (request.Subscribe && !cancel.IsCancellationRequested)
            {
                //Don't starve the request thread
                _logger.LogInformation("Not cancelled yet, going to sleep...");
                await Task.Delay(5000);
            }

            _logger.LogInformation("Done");
        }
    }
}
