using System.Collections.Generic;
using Common.Core;

namespace Instruments.Query.Service.Features.GetInstrumentsOverview
{
    public class Query : IQuery<Dto>
    {
        public string Vendor { get; set; }
    }
}
