using System;
using System.Collections.Generic;
using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPricesOverview
{
    public class Query : IQuery<IEnumerable<Dto>>
    {
        public DateTime? AsOfDate { get; set; }
        public DateTime? AsAtDate { get; set; }

    }
}
