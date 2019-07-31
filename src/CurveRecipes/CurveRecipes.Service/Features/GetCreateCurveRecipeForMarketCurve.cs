using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features
{
    public class GetCreateCurveRecipeForMarketCurve : IQuery<GetCreateCurveRecipeDto>
    {
        public class Handler : IHandleQuery<GetCreateCurveRecipeForMarketCurve, GetCreateCurveRecipeDto>
        {
            private readonly IReadModelRepository<MarketCurveDto> _readModelRepository;

            public Handler(IReadModelRepository<MarketCurveDto> readModelRepository)
            {
                _readModelRepository = readModelRepository ?? throw new ArgumentNullException(nameof(readModelRepository));
            }

            public async Task<GetCreateCurveRecipeDto> Handle(GetCreateCurveRecipeForMarketCurve query, CancellationToken cancellationToken)
            {
                var curves = await _readModelRepository.GetAll();

                return new GetCreateCurveRecipeDto
                {
                    Command = new CreateCurveRecipe
                    {
                        Id = Guid.NewGuid()
                    },
                    MarketCurves = curves.Where(x=> x.Tenors.Any())
                };
            }
        }
    }

    public class GetCreateCurveRecipeDto
    {
        public CreateCurveRecipe Command { get; set; }
        public IEnumerable<MarketCurveDto> MarketCurves { get; set; } = new List<MarketCurveDto>();
        public IEnumerable<string> DayCountConventions { get; set; } = Enum.GetNames(typeof(DayCountConvention));
        public IEnumerable<string> Interpolations { get; set; } = Enum.GetNames(typeof(Interpolation));
        public IEnumerable<string> ExtrapolationShorts { get; set; } = Enum.GetNames(typeof(ExtrapolationShort));
        public IEnumerable<string> ExtrapolationLongs { get; set; } = Enum.GetNames(typeof(ExtrapolationLong));
        public IEnumerable<string> OutputTypes { get; set; } = Enum.GetNames(typeof(OutputType));
        public IEnumerable<string> OutputSeries { get; set; } = Enum.GetNames(typeof(OutputSeries));
    }
}
