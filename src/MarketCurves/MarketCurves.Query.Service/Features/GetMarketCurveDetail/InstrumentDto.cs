using Common.Core;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class InstrumentDto : ReadObject
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
    }
}
