using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace CurveRecipes.Service.Features
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
        public async Task<IActionResult> GetCreateCurveRecipe()
        {
            var result = await _requestMediator.Send(new Query());
            return this.ComponentActionResult(result, "create-curve-recipe");
        }

        [HttpPost]
        public async Task<ActionResult> CreateCurveRecipe([FromBody] Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id}/addtransformation")]
        public async Task<IActionResult> GetAddTransformation([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new Query{ RecipeId = id });
            return this.ComponentActionResult(result, "add-transformation");
        }

        [HttpPost("addparallelshock")]
        public async Task<ActionResult> AddParallelShock([FromBody] Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("addkeyrateshock")]
        public async Task<ActionResult> AddKeyRateShock([FromBody] Command command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }
    }
}
