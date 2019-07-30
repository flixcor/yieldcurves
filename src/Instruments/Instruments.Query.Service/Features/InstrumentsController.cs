using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Core;
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
        public async Task<ActionResult<IEnumerable<InstrumentDto>>> Get()
        {
            var result = await _mediator.Send(new GetInstrumentList());
            return Ok(result);
        }
    }
}
