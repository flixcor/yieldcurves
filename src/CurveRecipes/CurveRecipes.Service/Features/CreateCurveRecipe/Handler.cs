using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Events;
using Common.Core.Extensions;
using CurveRecipes.Domain;
using Common.Infrastructure.Extensions;
using System.Linq;
using Common.EventStore.Lib;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class CommandHandler :
            IHandleCommand<Command>,
            IHandleQuery<Query, Dto>,
            IHandleEvent<IMarketCurveCreated>,
            IHandleEvent<ICurvePointAdded>
    {
        private readonly IAggregateRepository _repository;
        private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

        public CommandHandler(IAggregateRepository repository, IReadModelRepository<MarketCurveDto> readModelRepository)
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
                       var recipeResult = new CurveRecipe().TryDefine(command.MarketCurveId, command.ShortName, command.Description, command.LastLiquidTenor, command.DayCountConvention, command.Interpolation, command.ExtrapolationShort,
                           command.ExtrapolationLong, outputFrequency, command.OutputType);

                       return recipeResult.Promise(r => _repository.Save(r));
                   });
               });
        }

        public Task Handle(IEventWrapper<IMarketCurveCreated> @event, CancellationToken cancellationToken)
        {
            var curve = new MarketCurveDto
            {
                Id = @event.AggregateId,
                Name = GenerateName(@event.Content)
            };

            return _readModelRepository.Insert(curve);
        }

        public async Task Handle(IEventWrapper<ICurvePointAdded> @event, CancellationToken cancellationToken)
        {
            var curve = await _readModelRepository.Get(@event.AggregateId);

            await curve
                .ToResult()
                .Promise(x =>
                {
                    x.Tenors.Add(@event.Content.Tenor);
                    return _readModelRepository.Update(x);
                });
        }

        public async Task<Dto> Handle(Query query, CancellationToken cancellationToken)
        {
            var curves = await _readModelRepository.GetMany(x => x.Tenors.Any()).ToListAsync(cancellationToken);

            return new Dto
            {
                Command = new Command
                {
                    Id = Guid.NewGuid()
                },
                MarketCurves = curves
            };
        }

        private string GenerateName(IMarketCurveCreated @event)
        {
            var stringBuilder = new StringBuilder("M");

            stringBuilder.AppendFormatNonEmptyString("_{0}", @event.Country, @event.CurveType, @event.FloatingLeg);

            return stringBuilder.ToString();
        }
    }
}
