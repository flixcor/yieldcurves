﻿using System.IO;
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

            using var member = new StreamWriter(Response.Body);

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

                await member.WriteLineAsync($"event: {type}\ndata: {json}\n\n");
                await member.FlushAsync();
            }

            await subscriber.Subscribe(request.PreparePosition, request.CommitPosition, true, OnEvent, cancel);

            try
            {
                await Task.Delay(Timeout.Infinite, cancel);
            }
            catch (TaskCanceledException)
            {
            }

            _logger.LogInformation("done");
        }
    }
}