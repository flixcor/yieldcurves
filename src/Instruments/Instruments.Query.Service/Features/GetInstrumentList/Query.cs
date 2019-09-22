using System.Collections.Generic;
using Common.Core;

namespace Instruments.Query.Service.Features.GetInstrumentList
{
    public class Query : IQuery<IEnumerable<Dto>>
    {
        public string Vendor { get; set; }
    }
}
