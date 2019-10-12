using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace PricePublisher.Query.Service.Features.Common
{
    [ApiController]
    [Route("api")]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;

        public Controller(IRequestMediator requestMediator)
        {
            _requestMediator = requestMediator;
        }

        [HttpGet]
        public async Task<IActionResult> Handle([FromQuery] GetPricesOverview.Query query)
        {
            var result = await _requestMediator.Send(query);
            return this.ComponentActionResult(result, "get-price-list");
        }

        [HttpGet("get-price-dates")]
        public async Task<IActionResult> Handle([FromQuery] GetPriceDates.Query query)
        {
            var result = await _requestMediator.Send(query);
            return Ok(result);
        }
    }
}
