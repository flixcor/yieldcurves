using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Core.Extensions;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class CommandHandler :
            IHandleCommand<Command>,
            IHandleEvent<MarketCurveCreated>,
            IHandleEvent<CurvePointAdded>
    {
        private readonly IRepository _repository;
        private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

        public CommandHandler(IRepository repository, IReadModelRepository<MarketCurveDto> readModelRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return _readModelRepository
               .Get(command.MarketCurveId)
               .ToResult()
               .Promise(c =>
               {
                   var maturityResult = Maturity.TryCreate(command.OutputFrequency.MaximumMaturity);
                   return maturityResult.Promise(m =>
                   {
                       var outputFrequency = new Domain.OutputFrequency(command.OutputFrequency.OutputSeries, m);
                       var recipeResult = CurveRecipe.TryCreate(command.Id, command.MarketCurveId, command.ShortName, command.Description, command.LastLiquidTenor, command.DayCountConvention, command.Interpolation,
                       command.ExtrapolationShort, command.ExtrapolationLong, outputFrequency, command.OutputType);

                       return recipeResult.Promise(r => _repository.SaveAsync(r));
                   });
               });
        }

        public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
        {
            var curve = new MarketCurveDto
            {
                Id = @event.AggregateId,
                Name = GenerateName(@event)
            };

            return _readModelRepository.Insert(curve);
        }

        public async Task Handle(CurvePointAdded @event, CancellationToken cancellationToken)
        {
            var curve = await _readModelRepository.Get(@event.AggregateId);

            await curve
                .ToResult()
                .Promise(x =>
                {
                    x.Tenors.Add(@event.Tenor);
                    return _readModelRepository.Update(x);
                });
        }

        private string GenerateName(MarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder("M");

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }
    }
}
