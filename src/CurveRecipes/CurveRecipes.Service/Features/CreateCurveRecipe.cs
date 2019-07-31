using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Common.Core.Events;
using Common.Core.Extensions;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features
{
    public class OutputFrequency
    {
        public OutputSeries OutputSeries { get; set; }
        public double MaximumMaturity { get; set; }
    }

    public class CreateCurveRecipe : ICommand
    {
        public Guid Id { get; set; }
        public Guid MarketCurveId { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public Tenor LastLiquidTenor { get; set; }
        public DayCountConvention DayCountConvention { get; set; }
        public Interpolation Interpolation { get; set; }
        public ExtrapolationShort ExtrapolationShort { get; set; }
        public ExtrapolationLong ExtrapolationLong { get; set; }
        public OutputFrequency OutputFrequency { get; set; }
        public OutputType OutputType { get; set; }



        public class Handler :
            IHandleCommand<CreateCurveRecipe>,
            IHandleEvent<MarketCurveCreated>,
            IHandleEvent<CurvePointAdded>
        {
            private readonly IRepository _repository;
            private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

            public Handler(IRepository repository, IReadModelRepository<MarketCurveDto> readModelRepository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public Task<Result> Handle(CreateCurveRecipe command, CancellationToken cancellationToken)
            {
                 return _readModelRepository
                    .Get(command.MarketCurveId)
                    .ToResult()
                    .Promise(c =>
                    {
                        var outputFrequency = new Domain.OutputFrequency(command.OutputFrequency.OutputSeries, new Maturity(command.OutputFrequency.MaximumMaturity));
                        var recipe = new CurveRecipe(command.Id, command.MarketCurveId, command.ShortName, command.Description, command.LastLiquidTenor, command.DayCountConvention, command.Interpolation,
                        command.ExtrapolationShort, command.ExtrapolationLong, outputFrequency, command.OutputType);
                        return _repository.SaveAsync(recipe);
                    });
            }

            public Task Handle(MarketCurveCreated @event, CancellationToken cancellationToken)
            {
                var curve = new MarketCurveDto
                {
                    Id = @event.Id,
                    Name = GenerateName(@event)
                };

                return _readModelRepository.Insert(curve);
            }

            public async Task Handle(CurvePointAdded @event, CancellationToken cancellationToken)
            {
                var curve = await _readModelRepository.Get(@event.Id);

                await curve
                    .ToResult()
                    .Promise(x=> {
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

    public class MarketCurveDto : ReadObject
    {
        public string Name { get; set; }
        public IList<string> Tenors { get; set; } = new List<string>();
    }
}
