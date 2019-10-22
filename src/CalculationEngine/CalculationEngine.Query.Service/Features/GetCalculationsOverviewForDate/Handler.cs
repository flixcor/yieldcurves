using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;

namespace CalculationEngine.Query.Service.Features.GetCalculationsOverviewForDate
{
    public class Handler :
        IHandleEvent<CurveCalculated>,
        IHandleQuery<Query, Maybe<Dto>>,
        IHandleEvent<CurveRecipeCreated>
    {
        private readonly IReadModelRepository<Dto> _dtoRepository;
        private readonly IReadModelRepository<RecipeDto> _recipeRepository;

        public Handler(IReadModelRepository<Dto> dtoRepository, IReadModelRepository<RecipeDto> recipeRepository)
        {
            _dtoRepository = dtoRepository ?? throw new ArgumentNullException(nameof(dtoRepository));
            _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
        }

        public async Task Handle(CurveCalculated @event, CancellationToken cancellationToken)
        {
            var asOfDate = @event.AsOfDate.ToString("yyyy-MM-dd");

            var recipe = (await _recipeRepository.Single(x => x.Id == @event.CurveRecipeId)).Coalesce(new RecipeDto
            {
                Id = @event.CurveRecipeId,
                Name = "unknown"
            });

            var existingDto = await _dtoRepository.Single(x => x.AsOfDate == asOfDate);

            if (!existingDto.Found)
            {
                var dto = new Dto
                {
                    Id = Guid.NewGuid(),
                    AsOfDate = asOfDate,
                    Recipes = new RecipeDto[] { recipe }
                };

                await _dtoRepository.Insert(dto);
            }

            else
            {
                var dto = existingDto.ToResult().Content;
                var recipes = dto.Recipes.ToList();
                recipes.Add(recipe);
                dto.Recipes = recipes;

                await _dtoRepository.Update(dto);
            }
        }

        public Task Handle(CurveRecipeCreated @event, CancellationToken cancellationToken)
        {
            return _recipeRepository.Insert(new RecipeDto
            {
                Id = @event.Id,
                Name = @event.ShortName
            });
        }

        public Task<Maybe<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return _dtoRepository.Single(x => x.AsOfDate == query.AsOfDate);
        }
    }
}
