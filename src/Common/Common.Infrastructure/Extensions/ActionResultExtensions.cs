﻿using Common.Core;
using Microsoft.AspNetCore.Mvc;

namespace Common.Infrastructure.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult ToActionResult(this Result result)
        {
            return result.IsSuccessful
                ? (ActionResult)new OkResult()
                : new BadRequestObjectResult(result.Messages);
        }

        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            return result.IsSuccessful
                ? (ActionResult)new OkObjectResult(result.Content)
                : new BadRequestObjectResult(result.Messages);
        }

        public static ActionResult<T> ToActionResult<T>(this Maybe<T> maybe) where T : class
        {
            return maybe.Found
                ? maybe.ToResult().ToActionResult()
                : new NotFoundResult();
        }

        public static ActionResult<T> ToComponentActionResult<T>(this Maybe<T> maybe, string url) where T : class
        {
            return maybe.Found
                ? (ActionResult<T>) new OkObjectResult(FrontendComponent.Create(maybe.ToResult().Content, url))
                : new NotFoundResult();
        }
    }
}
