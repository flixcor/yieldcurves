using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features
{
    public class GetAddTransformation : IQuery<GetAddTransformation.Result>
    {
        public Guid RecipeId { get; set; }

        public class Result
        {
            public Result(Guid id)
            {
                Commands = new Dictionary<string, ICommand>()
                {
                    { nameof(KeyRateShock), new AddKeyRateShock { Id = id } },
                    { nameof(ParallelShock), new AddParallelShock { Id = id } }
                }.ToImmutableDictionary();
            }

            public ImmutableArray<string> ShockTargets { get; } = Enum.GetNames(typeof(ShockTarget)).ToImmutableArray();

            public ImmutableDictionary<string, ICommand> Commands { get; } = ImmutableDictionary<string, ICommand>.Empty;
        }

        public class Handler : IHandleQuery<GetAddTransformation, GetAddTransformation.Result>
        {
            public Task<GetAddTransformation.Result> Handle(GetAddTransformation query, CancellationToken cancellationToken)
            {
                var result = new GetAddTransformation.Result(query.RecipeId);
                return Task.FromResult(result);
            }
        }
    }
}
