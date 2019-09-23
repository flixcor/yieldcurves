using System;
using Common.Core;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Query : IQuery<Maybe<Dto>>
    {
        public Query(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }


    }


}
