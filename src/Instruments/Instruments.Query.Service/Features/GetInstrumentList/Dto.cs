using Common.Core;

namespace Instruments.Query.Service.Features.GetInstrumentList
{
    public class Dto : ReadObject
    {
        public string Vendor { get; set; }
        public string Description { get; set; }
    }
}
