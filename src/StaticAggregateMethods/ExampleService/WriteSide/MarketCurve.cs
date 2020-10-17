using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Aggregates;
using static Lib.Domain.MarketCurve.Events;
using Contracts;

namespace Lib.Domain
{
    public static class MarketCurve
    {
        public class Aggregate : Aggregate<State>
        {
            public Aggregate()
            {
                Handle<NameAndAddInstrument>((_, command) => Yield(new MarketCurveNamed(command.Name), new InstrumentAddedToCurve(command.Instrument)));

                Handle<AddInstrument>((state, command) => state.Instruments.Contains(command.Instrument)
                    ? None
                    : Yield(new InstrumentAddedToCurve(command.Instrument)));


                When<MarketCurveNamed>((state, @event) => state with { Name = @event.Name });
                When<InstrumentAddedToCurve>((state, @event) => state with { Instruments = state.Instruments.Append(@event.InstrumentId).ToList() });
            }

            public override string GetStreamName(string id) => "marketcurve-" + id;
            public override State InitState() => new State(string.Empty, Array.Empty<string>());
        }

        public record State(string Name, IReadOnlyCollection<string> Instruments);

        public static class Events
        {
            public record MarketCurveNamed(string Name);

            public record InstrumentAddedToCurve(string InstrumentId);

            public static readonly IEnumerable<object> None = Enumerable.Empty<object>();
            public static IEnumerable<object> Yield(params object?[] events) => events.Where(x=> x != null)!;
        }
    }


}
