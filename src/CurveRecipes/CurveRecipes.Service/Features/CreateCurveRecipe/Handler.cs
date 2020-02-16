using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Extensions;
using Common.Events;
using Common.Infrastructure;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class CommandHandler :
        ApplicationService<CurveRecipe>,
        IHandleCommand<Command>,
        IHandleQuery<Query, Dto>,
        IHandleEvent<IMarketCurveCreated>,
        IHandleEvent<ICurvePointAdded>
    {
        private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

        public CommandHandler(IAggregateRepository repository, IReadModelRepository<MarketCurveDto> readModelRepository) : base(repository)
        {
            _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken) 
            => Maturity.TryCreate(command.OutputFrequency.MaximumMaturity).Promise(m =>
            {
                var outputFrequency = new Domain.OutputFrequency(command.OutputFrequency.OutputSeries, m);

                return Handle(cancellationToken, command.Id.NonEmpty(), whatToDo:
                    curveRecipe => curveRecipe.Define(
                        marketCurveId: command.MarketCurveId.NonEmpty(),
                        shortName: command.ShortName.NonEmpty(),
                        description: command.Description.NonEmpty(),
                        lastLiquidTenor: command.LastLiquidTenor,
                        dayCountConvention: command.DayCountConvention,
                        interpolation: command.Interpolation,
                        extrapolationShort: command.ExtrapolationShort,
                        extrapolationLong: command.ExtrapolationLong,
                        outputFrequency: outputFrequency,
                        outputType: command.OutputType)
                    );
            });

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
