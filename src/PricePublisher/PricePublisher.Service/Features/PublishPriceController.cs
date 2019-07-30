using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using PricePublisher.Service.Features;

namespace PricePublisher.Service.Controllers
{
    [Route("api")]
    [ApiController]
    public class PublishPriceController : ControllerBase
    {
        private readonly IRequestMediator _mediator;

        public PublishPriceController(IRequestMediator mediator)
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PublishPrice command)
        {
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
