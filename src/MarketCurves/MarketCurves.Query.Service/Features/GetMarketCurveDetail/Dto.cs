using System.Collections.Generic;
using Common.Core;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class Dto : ReadObject
    {
        public string Name { get; set; }
        public IEnumerable<PointDto> CurvePoints { get; set; } = new List<PointDto>();
    }
}
