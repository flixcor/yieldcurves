using Common.Core;

namespace PricePublisher.Service.Features.PublishPrice
{
    public class InstrumentDto : ReadObject
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
        public bool HasPriceType { get; set; }
    }
}
