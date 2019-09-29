using System;
using System.Collections.Generic;
using Common.Core;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class UsedValues : ReadObject
    {
        public IList<Guid> Instruments { get; set; } = new List<Guid>();
        public IList<string> Tenors { get; set; } = new List<string>();
    }
}
