using System;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Command : ICommand
    {
        public Guid MarketCurveId { get; set; }
        public Tenor Tenor { get; set; }
        public Guid InstrumentId { get; set; }
        public short DateLag { get; set; }
        public bool IsMandatory { get; set; }
        public PriceType? PriceType { get; set; }
    }
}
