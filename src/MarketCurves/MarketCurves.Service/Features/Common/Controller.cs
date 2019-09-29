using System;
using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketCurves.Service.Features
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _commandMediator;

        public Controller(IRequestMediator commandMediator)
        {
            _commandMediator = commandMediator ?? throw new ArgumentNullException(nameof(commandMediator));
        }

        [HttpGet]
        public async Task<IActionResult> CreateMarketCurve()
        {
            var result = await _commandMediator.Send(new CreateMarketCurve.Query());
            return this.ComponentActionResult(result, "create-market-curve");
        }

        [HttpPost]
        public async Task<ActionResult> CreateMarketCurve(CreateMarketCurve.Command command)
        {
            var result = await _commandMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("curvepoint")]
        public async Task<ActionResult> AddCurvePoint(AddCurvePoint.Command addCurvePointCommand)
        {
            var result = await _commandMediator.Send(addCurvePointCommand);
            return result.ToActionResult();
        }

        [HttpGet("curvepoint/{id}")]
        public async Task<IActionResult> AddCurvePoint([FromRoute] Guid id)
        {
            var result = await _commandMediator.Send(new AddCurvePoint.Query { MarketCurveId = id });
            return this.ComponentActionResult(result, "add-curve-point");
        }
    }
}
