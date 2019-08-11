using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketCurves.Service.Features
{
    [Route("api")]
    [ApiController]
    public class MarketCurveController : ControllerBase
    {
        private readonly IRequestMediator _commandMediator;

        public MarketCurveController(IRequestMediator commandMediator)
        {
            _commandMediator = commandMediator ?? throw new ArgumentNullException(nameof(commandMediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetCreateCommand()
        {
            var result = await _commandMediator.Send(new GetCreateMarketCurve());
            return this.ComponentActionResult(result, "create-market-curve");
        }

        [HttpPost]
        public async Task<ActionResult> CreateMarketCurve(CreateMarketCurve command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("curvepoint")]
        public async Task<ActionResult> AddCurvePoint(AddCurvePoint command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("curvepoint/{id}")]
        public async Task<IActionResult> AddCurvePoint([FromRoute] Guid id)
        {
            var result = await _commandMediator.Send(new GetAddCurvePoint { MarketCurveId = id });
            return this.ComponentActionResult(result, "add-curve-point");
        }
    }
}
