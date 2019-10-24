using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CalculationEngine.Query.Service.Features.GetCalculationsOverviewForDate
{
    public class Handler :
        IHandleEvent<CurveCalculated>,
        IHandleQuery<Query, Maybe<Dto>>,
        IHandleEvent<CurveRecipeCreated>
    {
        private readonly IReadModelRepository<Dto> _dtoRepository;
        private readonly IReadModelRepository<RecipeDto> _recipeRepository;
        private readonly GenericDbContext _genericDbContext;

        public Handler(IReadModelRepository<Dto> dtoRepository, IReadModelRepository<RecipeDto> recipeRepository, GenericDbContext genericDbContext)
        {
            _dtoRepository = dtoRepository ?? throw new ArgumentNullException(nameof(dtoRepository));
            _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
            _genericDbContext = genericDbContext;
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
                Id = @event.AggregateId,
                Name = @event.ShortName
            });
        }

        public async Task<Maybe<Dto>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _genericDbContext
                .Set<Dto>()
                .Include(x => x.Recipes)
                .FirstOrDefaultAsync(x => x.AsOfDate == query.AsOfDate);

            return result.Maybe();
        }
    }
}
