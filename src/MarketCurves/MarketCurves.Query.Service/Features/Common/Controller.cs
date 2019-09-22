using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MarketCurves.Query.Service.Features.Common
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;

        public Controller(IRequestMediator requestMediator)
        {
            _requestMediator = requestMediator ?? throw new ArgumentNullException(nameof(requestMediator));
        }

        // GET api/values
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get([FromQuery] GetMarketCurvesOverview.Query query)
        {
            var result = await _requestMediator.Send(query);
            return this.HubComponentActionResult(result, "get-market-curves");
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new GetMarketCurveDetail.Query { Id = id });
            return this.ComponentActionResult(result, "get-market-curve");
        }
    }
}
