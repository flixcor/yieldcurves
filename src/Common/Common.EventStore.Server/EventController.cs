using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Core;
using Common.EventStore.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Common.EventStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IEventReadRepository _repository;
        private static readonly JsonSerializerOptions s_jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public EventController(ILogger<EventController> logger, IEventReadRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public IAsyncEnumerable<EventReply> GetAsync([FromQuery]EventRequest request) =>
            _repository.Get(
                eventFilter: request,
                cancellation: HttpContext.RequestAborted
            )
                .Select(t =>
                {
                    var (e, _) = t;
                    var content = e.GetContent();

                    return new EventReply
                    {
                        Position = e.Id,
                        Type = content.GetType().Name,
                        Payload = content
                    };
                });

        [HttpGet("subscribe")]
        public async Task GetEvents([FromQuery]EventRequest request)
        {
            var cancel = HttpContext.RequestAborted;

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";

            var lastEventId = Request.Headers["Last-Event-ID"].ToString();

            if (long.TryParse(lastEventId, out var checkpoint))
            {
                request.Checkpoint = checkpoint;
            }

            var writer = new StreamWriter(Response.Body);

            var counter = 0;

            try
            {
                await foreach (var wrapper in _repository.Get(request, cancel))
                {
                    counter++;

                    await WriteEvent(writer, wrapper);

                    if (counter % 500 == 0)
                    {
                        await writer.FlushAsync();
                    }

                    request.Checkpoint = wrapper.Item1.Id;
                }

                await writer.WriteLineAsync("event: sync\n\n");
                await writer.FlushAsync();

                await foreach (var tup in _repository.Subscribe(request, cancel))
                {
                    if (!request.Checkpoint.HasValue || request.Checkpoint.Value < tup.Item1.Id)
                    {
                        await WriteEvent(writer, tup);
                        await writer.FlushAsync();
                    }
                }
            }
            catch (System.OperationCanceledException)
            {
                _logger.LogInformation("cancelled");
            }

            _logger.LogInformation("done");
        }

        private async Task WriteEvent(StreamWriter writer, (IEventWrapper, IMetadata) tup)
        {
            var (wrapper, _) = tup;

            var payload = wrapper.GetContent();
            var type = payload.GetType();
            var name = type.Name;
            var position = wrapper.Id;

            var json = JsonSerializer.Serialize(payload, type, s_jsonSerializerOptions);

            _logger.LogInformation($"returning event: {name}");

            await writer.WriteLineAsync($"id: {position}\nevent: {name}\ndata: {json}\n\n");
        }
    }
}

