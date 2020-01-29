using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
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

        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };


        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IAsyncEnumerable<EventReply> GetAsync([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;
            var query = new EventStoreQuery("ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500");

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    query.RegisterEventType(type);
                }
            }

            return query.Run(cancel).Select(e=> new EventReply 
            { 
                Position = e.Item1,
                Type = e.Item2,
                Payload = e.Item3
            });
        }

        [HttpGet("subscribe")]
        public async Task GetEvents([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";

            var writer = new StreamWriter(Response.Body);

            var subscriber = new EventStoreSocketSubscriber("ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500");

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    subscriber.RegisterEventType(type);
                }
            }

            async Task OnEvent(IEvent payload, string type, long commitPosition)
            {
                var json = JsonSerializer.Serialize(payload, payload?.GetType(), s_jsonSerializerOptions);

                _logger.LogInformation($"returning event: {type}");

                await writer.WriteLineAsync($"id: {commitPosition}\nevent: {type}\ndata: {json}\n\n");
                await writer.FlushAsync();
            }

            try
            {
                await subscriber.Subscribe(request.Position, OnEvent, cancel);
                await Task.Delay(Timeout.Infinite, cancel);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("cancelled");
            }

            _logger.LogInformation("done");
        }
    }
}
