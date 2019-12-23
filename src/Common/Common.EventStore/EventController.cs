using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.EventStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task GetEvents([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";

            var writer = new StreamWriter(Response.Body);

            using var subscriber = new EventStoreSocketSubscriber("ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500");

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    subscriber.RegisterEventType(type);
                }
            }

            async Task OnEvent(byte[] payload, string type, long preparePosition, long commitPosition)
            {
                var reply = new EventReply
                {
                    PreparePosition = preparePosition,
                    CommitPosition = commitPosition,
                    EventType = type,
                    Payload = payload
                };

                var json = JsonSerializer.Serialize(reply);

                _logger.LogInformation($"returning event: {type}");

                await writer.WriteLineAsync($"event: {type}\ndata: {json}\n\n");
                await writer.FlushAsync();
            }

            try
            {
                await subscriber.Subscribe(request.PreparePosition, request.CommitPosition, true, OnEvent, cancel);

                do
                {
                    await Task.Delay(30000, cancel);
                    await writer.WriteLineAsync("...");
                    await writer.FlushAsync();                   
                } while (!cancel.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("cancelled");
            }

            _logger.LogInformation("done");
        }
    }

    internal static class TaskHelper
    {
        #region Methods
        internal static Task WaitAsync(this CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> cancelationTaskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(CancellationTokenCallback, cancelationTaskCompletionSource);

            return cancellationToken.IsCancellationRequested ? Task.CompletedTask : cancelationTaskCompletionSource.Task;
        }

        private static void CancellationTokenCallback(object taskCompletionSource)
        {
            ((TaskCompletionSource<bool>)taskCompletionSource).SetResult(true);
        }
        #endregion
    }
}
