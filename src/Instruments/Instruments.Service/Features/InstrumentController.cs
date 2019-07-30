using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Instruments.Service.Features;
using Microsoft.AspNetCore.Mvc;

namespace Instruments.Service.Application
{
    [Route("api")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        private readonly IRequestMediator _commandMediator;

        public InstrumentController(IRequestMediator commandMediator)
        {
            _commandMediator = commandMediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCommands()
        {
            var result = await _commandMediator.Send(new GetCommands());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRegularInstrument(CreateRegularInstrument command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("bloomberg")]
        public async Task<ActionResult> CreateBloombergInstrument(CreateBloombergInstrument command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }
    }
}
