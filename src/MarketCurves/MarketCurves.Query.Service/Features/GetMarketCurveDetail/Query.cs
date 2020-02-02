using System;
using Common.Core;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class Query : IQuery<Dto?>
    {
        public Guid Id { get; set; }
    }
}
