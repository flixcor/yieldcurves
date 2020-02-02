using System;
using Common.Core;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Query : IQuery<Dto?>
    {
        public Guid Id { get; set; }
    }
}
