using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Instruments.Query.Service.Features
{
    [Route("api")]
    [ApiController]
    public class InstrumentsController : ControllerBase
    {
        private readonly IRequestMediator _mediator;

        public InstrumentsController(IRequestMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // GET api
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new GetInstrumentList();
            var result = await _mediator.Send(query);
            return this.ComponentActionResult(result, "get-instrument-list");
        }
    }
}
