using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure;
using EventStore.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.EventStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly EventStoreClient _client;
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };


        public EventController(ILogger<EventController> logger, EventStoreClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public IAsyncEnumerable<EventReply> GetAsync([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;
            var query = new EventStoreQuery(_client);

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    query.RegisterEventType(type);
                }
            }

            return query.Run(cancel).Select(e => new EventReply
            {
                Position = e.Id,
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

            var subscriber = new EventStoreSocketSubscriber(_client);

            if (request.EventTypes != null)
            {
                foreach (var type in request?.EventTypes)
                {
                    subscriber.RegisterEventType(type);
                }
            }

            async Task OnEvent(IEventWrapper wrapper)
            {
                var payload = wrapper.Content;
                var type = payload.GetType().Name;
                var position = wrapper.Id;

                var json = JsonSerializer.Serialize(payload, payload?.GetType(), s_jsonSerializerOptions);

                _logger.LogInformation($"returning event: {type}");

                await writer.WriteLineAsync($"id: {position}\nevent: {type}\ndata: {json}\n\n");
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
