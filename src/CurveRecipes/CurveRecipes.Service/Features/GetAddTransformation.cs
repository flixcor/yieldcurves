using System;
using System.Collections.Generic;
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
                Commands.Add(nameof(KeyRateShock), new AddKeyRateShock { Id = id });
                Commands.Add(nameof(ParallelShock), new AddParallelShock { Id = id });
            }

            public string[] ShockTargets { get; } = Enum.GetNames(typeof(ShockTarget)).ToArray();

            public Dictionary<string, ICommand> Commands { get; } = new Dictionary<string, ICommand>();
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
