using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features
{
    public class GetCreateMarketCurve : IQuery<CreateMarketCurveDto>
    {
        public class Handler : IHandleQuery<GetCreateMarketCurve, CreateMarketCurveDto>
        {
            public Task<CreateMarketCurveDto> Handle(GetCreateMarketCurve query, CancellationToken cancellationToken)
            {
                return Task.FromResult(new CreateMarketCurveDto());
            }
        }
    }

    public class CreateMarketCurveDto
    {
        public CreateMarketCurve Command { get; set; } = new CreateMarketCurve
        {
            Id = Guid.NewGuid()
        };

        public IEnumerable<string> Countries { get; set; } = Enum.GetNames(typeof(Country));
        public IEnumerable<string> CurveTypes { get; set; } = Enum.GetNames(typeof(CurveType));
        public IEnumerable<string> FloatingLegs { get; set; } = Enum.GetNames(typeof(FloatingLeg));
    }
}
