using System.Threading;
using System.Threading.Tasks;
using Common.Core;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Handler : IHandleQuery<Query, Dto>
    {
        public Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = new Dto(query.RecipeId);
            return Task.FromResult(result);
        }
    }
}
