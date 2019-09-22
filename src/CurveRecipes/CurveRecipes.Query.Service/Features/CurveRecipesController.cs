using System;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Extensions;

namespace CurveRecipes.Query.Service.Features
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

        // GET api
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _requestMediator.Send(new GetCurveRecipeList());
            return this.HubComponentActionResult(result, "get-curve-recipe-list");
        }

        // GET api/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new GetCurveRecipeDetail(id));
            return this.ComponentActionResult(result, "get-curve-recipe-detail");
        }
    }
}
