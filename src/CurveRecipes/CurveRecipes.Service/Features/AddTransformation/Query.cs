using System;
using Common.Core;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Query : IQuery<Dto>
    {
        public Guid RecipeId { get; set; }
    }
}
