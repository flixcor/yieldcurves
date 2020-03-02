using Common.Core;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Instrument : ReadObject
    {
        public string? Vendor { get; set; }
        public string? Name { get; set; }
        public bool HasPriceType { get; set; }
    }
}
