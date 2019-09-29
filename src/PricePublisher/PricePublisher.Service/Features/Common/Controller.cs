using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace PricePublisher.Service.Features.Common
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _mediator;

        public Controller(IRequestMediator mediator)
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PublishPrice.Command command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PublishPrice.Query query)
        {
            var result = await _mediator.Send(query);
            return this.ComponentActionResult(result, "publish-price");
        }
    }
}
