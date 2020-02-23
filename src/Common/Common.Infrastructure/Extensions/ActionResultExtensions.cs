using Common.Core;
using Microsoft.AspNetCore.Mvc;

namespace Common.Infrastructure.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult ToActionResult(this Result result) 
            => result.IsSuccessful
                ? (ActionResult)new OkResult()
                : new BadRequestObjectResult(result.Messages);

        public static ActionResult<T> ToActionResult<T>(this Either<Error, T> result) 
            => result
                .MapLeft(l => (ActionResult<T>)new BadRequestObjectResult(l.Messages))
                .Reduce(r => r);

        public static ActionResult ToActionResult(this Either<Error, Nothing> result) 
            => result
                .MapLeft(l => (ActionResult)new BadRequestObjectResult(l.Messages))
                .Reduce(r => new OkResult());
    }
}
