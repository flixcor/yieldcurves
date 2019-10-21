using System;
using Common.Core;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Query : IQuery<Maybe<Dto>>
    {
        public Guid Id { get; set; }
    }
}
