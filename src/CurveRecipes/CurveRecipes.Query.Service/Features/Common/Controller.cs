using System;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Extensions;

namespace CurveRecipes.Query.Service.Features.Common
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

        // GET api
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCurveRecipesOverview.Query query)
        {
            var result = await _requestMediator.Send(query);
            return this.HubComponentActionResult(result, "get-curve-recipe-list");
        }

        // GET api/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _requestMediator.Send(new GetCurveRecipeDetail.Query(id));
            return this.ComponentActionResult(result, "get-curve-recipe-detail");
        }
    }
}
