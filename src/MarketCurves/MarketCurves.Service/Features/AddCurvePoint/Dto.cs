using System;
using System.Collections.Generic;
using MarketCurves.Domain;

namespace MarketCurves.Service.Features.AddCurvePoint
{
    public class Dto
    {
        public Dto(Command command, IEnumerable<Instrument> instruments, IEnumerable<string> tenors)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Instruments = instruments ?? new List<Instrument>();
            Tenors = tenors ?? new List<string>();
        }

        public Command Command { get; }
        public IEnumerable<Instrument> Instruments { get; }
        public IEnumerable<string> Tenors { get; }
        public IEnumerable<string> PriceTypes { get; set; } = Enum.GetNames(typeof(PriceType));
    }
}
