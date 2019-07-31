﻿using Common.Core;
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

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> CreateCurveRecipe([FromBody] CreateCurveRecipe command)
        {
            var result = await _requestMediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<ActionResult<GetCreateCurveRecipeDto>> CreateCurveRecipe([FromQuery] GetCreateCurveRecipeForMarketCurve query)
        {
            var result = await _requestMediator.Send(query);
            return Ok(result);
        }

        // POST api/values
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
