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
        IHandleEvent<ICurveCalculated>,
        IHandleQuery<Query, Dto?>,
        IHandleEvent<ICurveRecipeCreated>
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

        public async Task Handle(IEventWrapper<ICurveCalculated> wrapper, CancellationToken cancellationToken)
        {
            var @event = wrapper.Content;

            var asOfDate = @event.AsOfDate;

            var recipe = (await _recipeRepository.Single(x => x.Id == @event.CurveRecipeId)) ?? new RecipeDto
            {
                Id = @event.CurveRecipeId,
                Name = "unknown"
            };

            var dto = await _dtoRepository.Single(x => x.AsOfDate == asOfDate);

            if (dto == null)
            {
                dto = new Dto
                {
                    Id = Guid.NewGuid(),
                    AsOfDate = asOfDate,
                    Recipes = new RecipeDto[] { recipe }
                };

                await _dtoRepository.Insert(dto);
            }

            else
            {
                var recipes = dto.Recipes.ToList();
                recipes.Add(recipe);
                dto.Recipes = recipes;

                await _dtoRepository.Update(dto);
            }
        }

        public Task Handle(IEventWrapper<ICurveRecipeCreated> @event, CancellationToken cancellationToken)
        {
            return _recipeRepository.Insert(new RecipeDto
            {
                Id = @event.AggregateId,
                Name = @event.Content.ShortName
            });
        }

        public Task<Dto?> Handle(Query query, CancellationToken cancellationToken)
        {
            return _genericDbContext
                .Set<Dto>()
                .Include(x => x.Recipes)
                .Where(x=> x.AsOfDate == query.AsOfDate)
                .FirstOrDefaultAsync<Dto?>();
        }
    }
}
