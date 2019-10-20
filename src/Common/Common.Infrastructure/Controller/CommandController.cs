using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Common.Infrastructure.Controller
{
    public class CommandController<TCommand> : ControllerBase where TCommand : ICommand
    {
        private readonly IHandleCommand<TCommand> _handler;

        public CommandController(IHandleCommand<TCommand> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken ct = default)
        {
            using var reader = new StreamReader(Request.Body);            
            var body = await reader.ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<TCommand>(body);

            var result = await _handler.Handle(command, ct);

            return result.ToActionResult();
        }
    }
}
