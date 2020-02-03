using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib.GES;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.EventStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly EventStoreQuery _query;
        private readonly EventStoreSocketSubscriber _subscriber;
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public EventController(ILogger<EventController> logger, EventStoreQuery query, EventStoreSocketSubscriber subscriber)
        {
            _logger = logger;
            _query = query;
            _subscriber = subscriber;
        }

        [HttpGet]
        public IAsyncEnumerable<EventReply> GetAsync([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    _query.RegisterEventType(type);
                }
            }

            return _query.Run(cancel).Select(e => new EventReply
            {
                Position = e.Metadata.Id,
                Type = e.Content.GetType().Name,
                Payload = e.Content
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

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    _subscriber.RegisterEventType(type);
                }
            }

            async Task OnEvent(IEventWrapper wrapper)
            {
                var payload = wrapper.Content;
                var type = payload.GetType().Name;
                var position = wrapper.Metadata.Id;

                var json = JsonSerializer.Serialize(payload, payload?.GetType(), s_jsonSerializerOptions);

                _logger.LogInformation($"returning event: {type}");

                await writer.WriteLineAsync($"id: {position}\nevent: {type}\ndata: {json}\n\n");
                await writer.FlushAsync();
            }

            try
            {
                await _subscriber.Subscribe(request.Position, OnEvent, cancel);
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
