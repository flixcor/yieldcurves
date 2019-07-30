using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<GetMarketCurveList.Dto>>> Get()
        {
            var result = await _requestMediator.Send(new GetMarketCurveList.Query());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<GetMarketCurve.Dto>> Get(Guid id)
        {
            var result = await _requestMediator.Send(new GetMarketCurve.Query { Id = id });
            return result.ToActionResult();
        }
    }
}
