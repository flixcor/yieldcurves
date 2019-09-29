using System;
using System.Collections.Generic;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Dto
    {
        public Command Command { get; set; }
        public IEnumerable<Instrument> Instruments { get; set; } = new List<Instrument>();
        public IEnumerable<string> Tenors { get; set; } = new List<string>();
        public IEnumerable<string> PriceTypes { get; set; } = Enum.GetNames(typeof(PriceType));
    }
}
