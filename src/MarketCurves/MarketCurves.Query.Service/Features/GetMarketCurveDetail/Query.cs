using System;
using Common.Core;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class Query : IQuery<Maybe<Dto>>
    {
        public Guid Id { get; set; }
    }
}
