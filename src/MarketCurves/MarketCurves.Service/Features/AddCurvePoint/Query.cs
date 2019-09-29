using System;
using Common.Core;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Query : IQuery<Dto>
    {
        public Guid MarketCurveId { get; set; }
    }
}
