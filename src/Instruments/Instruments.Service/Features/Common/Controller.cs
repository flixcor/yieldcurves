using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Instruments.Service.Features.Common
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _commandMediator;

        public Controller(IRequestMediator commandMediator)
        {
            _commandMediator = commandMediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommands([FromQuery] GetCommands.Query query)
        {
            var result = await _commandMediator.Send(query);
            return this.ComponentActionResult(result, "create-instrument");
        }

        [HttpPost]
        public async Task<ActionResult> CreateRegularInstrument(CreateRegularInstrument.Command command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("bloomberg")]
        public async Task<ActionResult> CreateBloombergInstrument(CreateBloombergInstrument.Command command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }
    }
}
