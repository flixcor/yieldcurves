using System;

namespace MarketCurves.Query.Service.Features.GetMarketCurveDetail
{
    public class PointDto
    {
        public Guid InstrumentId { get; set; }
        public string Tenor { get; set; }
        public string Vendor { get; set; }
        public string Name { get; set; }
        public int DateLag { get; set; }
        public bool IsMandatory { get; set; }
        public string PriceType { get; set; }
    }
}
