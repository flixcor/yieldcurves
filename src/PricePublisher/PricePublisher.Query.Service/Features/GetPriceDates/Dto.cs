using System;
using Common.Core;

namespace PricePublisher.Query.Service.Features.GetPriceDates
{
    public class Dto : ReadObject
    {
        public string AsOfDate { get; set; }
    }
}
