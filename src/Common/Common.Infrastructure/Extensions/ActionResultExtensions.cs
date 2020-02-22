using Common.Core;
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

        public static IActionResult ToActionResult<T>(this Either<Error, T> result)
        {
            return result
                .MapLeft(l => (IActionResult)new BadRequestObjectResult(l.Messages))
                .Reduce(r => new OkObjectResult(r));
        }
    }
}
