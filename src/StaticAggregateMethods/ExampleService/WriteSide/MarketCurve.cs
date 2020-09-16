﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Aggregates;
using Lib.AspNet;
using static Lib.Domain.MarketCurve.Commands;
using static Lib.Domain.MarketCurve.Events;

namespace Lib.Domain
{
    public static class MarketCurve
    {
        public class Aggregate : Aggregate<State>
        {
            public Aggregate()
            {
                StreamName((id) => "marketcurve-" + id);

                Handle<NameAndAddInstrument>((_, command) =>
                {
                    if (command.Name == null || command.Instrument == null)
                    {
                        return Enumerable.Empty<object>();
                    }

                    return new object[] { new MarketCurveNamed(command.Name), new InstrumentAddedToCurve(command.Instrument) };
                });

                Handle<AddInstrument>((s, e) => e.Instrument == null || s.Instruments.Contains(e.Instrument)
                    ? Enumerable.Empty<object>()
                    : new InstrumentAddedToCurve(e.Instrument).Yield());


                When<MarketCurveNamed>((state, @event) => state with { Name = @event.Name });
                When<InstrumentAddedToCurve>((state, @event) => state with { Instruments = state.Instruments.Append(@event.InstrumentId).ToList() });
            }
        }

        public record State
        {
            public IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();
            public string? Name { get; init; }
        }

        public static class Commands
        {
            public record NameAndAddInstrument
            {
                public string? Name { get; init; }
                public string? Instrument { get; init; }
            }

            public record AddInstrument
            {
                public string? Instrument { get; init; }
            }
        }

        public static class Events
        {
            public record MarketCurveNamed(string Name);

            public record InstrumentAddedToCurve(string InstrumentId);
        }
    }


}
