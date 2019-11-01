using System.Collections.Generic;
using Common.Core;

namespace Instruments.Query.Service.Features.GetInstrumentsOverview
{
    public class Query : IListQuery<Dto>
    {
        public string Vendor { get; set; }
    }
}
