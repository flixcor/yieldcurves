using System;
using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPricesOverview
{
    public class Dto : ReadObject
    {
        public DateTime AsOfDate { get; set; }
        public DateTime AsAtDate { get; set; }
        public string Vendor { get; set; }
        public string Instrument { get; set; }
        public string PriceType { get; set; }
        public string PriceCurrency { get; set; }
        public double PriceAmount { get; set; }
    }
}
