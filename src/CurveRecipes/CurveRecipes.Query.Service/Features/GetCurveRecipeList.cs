using Common.Core;
using Common.Core.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurveRecipes.Query.Service.Features
{
    public class GetCurveRecipeList : IQuery<GetCurveRecipeList.Result>
    {
        public class Result{
            public Result(IEnumerable<GetCurveRecipesDto> recipes)
            {
                Recipes = recipes ?? new List<GetCurveRecipesDto>();
            }

            public IEnumerable<GetCurveRecipesDto> Recipes { get; set; }
        }

        public class Handler :
            IHandleQuery<GetCurveRecipeList, Result>,
            IHandleEvent<CurveRecipeCreated>
        {
            private readonly IReadModelRepository<GetCurveRecipesDto> _readModelRepository;

            public Handler(IReadModelRepository<GetCurveRecipesDto> readModelRepository)
            {
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public Task Handle(CurveRecipeCreated @event, CancellationToken cancellationToken)
            {
                var dto = new GetCurveRecipesDto
                {
                    Id = @event.Id,
                    Name = @event.ShortName
                };

                return _readModelRepository.Insert(dto);
            }

            public async Task<Result> Handle(GetCurveRecipeList query, CancellationToken cancellationToken)
            {
                var recipes = await _readModelRepository.GetAll();
                return new Result(recipes);
            }
        }
    }

    public class GetCurveRecipesDto : ReadObject
    {
        public string Name { get; set; }
    }
}
