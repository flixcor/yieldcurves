using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace CurveRecipes.Service.Features
{
    [Route("api")]
    [ApiController]
    public class CurveRecipesController : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;

        public CurveRecipesController(IRequestMediator requestMediator)
        {
            _requestMediator = requestMediator ?? throw new ArgumentNullException(nameof(requestMediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetCreateCurveRecipe()
        {
            var result = await _requestMediator.Send(new GetCreateCurveRecipe());
            return this.ComponentActionResult(result, "create-curve-recipe");
        }

        [HttpPost]
        public async Task<ActionResult> CreateCurveRecipe([FromBody] CreateCurveRecipe command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id}/addtransformation")]
        public async Task<IActionResult> GetAddTransformation([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new GetAddTransformation{ RecipeId = id });
            return this.ComponentActionResult(result, "add-transformation");
        }

        [HttpPost("addparallelshock")]
        public async Task<ActionResult> AddParallelShock([FromBody] AddParallelShock command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("addkeyrateshock")]
        public async Task<ActionResult> AddKeyRateShock([FromBody] AddKeyRateShock command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }
    }
}
