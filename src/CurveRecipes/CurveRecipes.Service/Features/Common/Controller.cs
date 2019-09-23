using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace CurveRecipes.Service.Features.Common
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

        [HttpGet]
        public async Task<IActionResult> GetCreateCurveRecipe([FromQuery] CreateCurveRecipe.Query query)
        {
            var result = await _requestMediator.Send(query);
            return this.ComponentActionResult(result, "create-curve-recipe");
        }

        [HttpPost]
        public async Task<ActionResult> CreateCurveRecipe([FromBody] CreateCurveRecipe.Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id}/addtransformation")]
        public async Task<IActionResult> GetAddTransformation([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new AddTransformation.Query{ RecipeId = id });
            return this.ComponentActionResult(result, "add-transformation");
        }

        [HttpPost("addparallelshock")]
        public async Task<ActionResult> AddParallelShock([FromBody] AddTransformation.AddShock.AddParallelShock.Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("addkeyrateshock")]
        public async Task<ActionResult> AddKeyRateShock([FromBody] AddTransformation.AddShock.AddKeyRateShock.Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }
    }
}
