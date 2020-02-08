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
                Position = e.Id,
                Type = e.GetContent().GetType().Name,
                Payload = e.GetContent()
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
                    _query.RegisterEventType(type);
                }
            }

            var counter = 0;

            async Task WriteEvent(IEventWrapper wrapper)
            {
                var payload = wrapper.GetContent();
                var type = payload.GetType();
                var name = type.Name;
                var position = wrapper.Id;

                var json = JsonSerializer.Serialize(payload, type, s_jsonSerializerOptions);

                _logger.LogInformation($"returning event: {name}");

                await writer.WriteLineAsync($"id: {position}\nevent: {type}\ndata: {json}\n\n");
            }

            await foreach (var wrapper in _query.Run(cancel))
            {
                counter++;

                await WriteEvent(wrapper);

                if (counter % 500 == 0)
                {
                    await writer.FlushAsync();
                }
            }

            async Task OnEvent(IEventWrapper wrapper)
            {
                await WriteEvent(wrapper);
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

