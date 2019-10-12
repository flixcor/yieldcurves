using System;
using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPriceDates
{
    public class Dto : ReadObject
    {
        public DateTime AsOfDate { get; set; }
    }
}
