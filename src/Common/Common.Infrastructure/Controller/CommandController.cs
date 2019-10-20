using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get([FromBody] TCommand command, CancellationToken ct = default)
        {
            var result = await _handler.Handle(command, ct);

            return result.ToActionResult();
        }
    }
}
