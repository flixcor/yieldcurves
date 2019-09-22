using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Instruments.Query.Service.Features.Common
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _mediator;

        public Controller(IRequestMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // GET api
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetInstrumentList.Query query)
        {
            var result = await _mediator.Send(query);
            return this.HubComponentActionResult(result, "get-instrument-list");
        }
    }
}
