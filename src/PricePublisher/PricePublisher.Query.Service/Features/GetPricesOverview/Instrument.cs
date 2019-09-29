using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPricesOverview
{
    public class Instrument : ReadObject
    {
        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
