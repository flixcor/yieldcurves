using System;
using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPricesOverview
{
    public class Query : IListQuery<Dto>
    {
        public DateTime? AsOfDate { get; set; }
        public DateTime? AsAtDate { get; set; }
    }
}
