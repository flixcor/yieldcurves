using System;
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
                Handle<NameAndAddInstrument>((_, command) => new object[] { new MarketCurveNamed(command.Name), new InstrumentAddedToCurve(command.Instrument) });

                Handle<AddInstrument>((state, command) => state.Instruments.Contains(command.Instrument)
                    ? Enumerable.Empty<object>()
                    : new InstrumentAddedToCurve(command.Instrument).Yield());


                When<MarketCurveNamed>((state, @event) => state with { Name = @event.Name });
                When<InstrumentAddedToCurve>((state, @event) => state with { Instruments = state.Instruments.Append(@event.InstrumentId).ToList() });
            }

            public override string GetStreamName(string id) => "marketcurve-" + id;
            public override State InitState() => new State(string.Empty, Array.Empty<string>());
        }

        public record State(string Name, IReadOnlyCollection<string> Instruments);

        public static class Commands
        {
            public record NameAndAddInstrument(string Name, string Instrument);

            public record AddInstrument(string Instrument);
        }

        public static class Events
        {
            public record MarketCurveNamed(string Name);

            public record InstrumentAddedToCurve(string InstrumentId);
        }
    }


}
