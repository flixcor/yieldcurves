using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MarketCurves.Query.Service.Features
{
    [Route("api")]
    [ApiController]
    public class MarketCurveController : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;

        public MarketCurveController(IRequestMediator requestMediator)
        {
            _requestMediator = requestMediator ?? throw new ArgumentNullException(nameof(requestMediator));
        }

        // GET api/values
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get()
        {
            var result = await _requestMediator.Send(new GetMarketCurveList.Query());
            return this.ComponentActionResult(result, "get-market-curves");
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _requestMediator.Send(new GetMarketCurve.Query { Id = id });
            return this.HubComponentActionResult(result, "get-market-curve");
        }
    }
}
