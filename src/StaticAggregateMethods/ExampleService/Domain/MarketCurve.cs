using System.Collections.Generic;
using System.Linq;
using static ExampleService.Domain.Commands;
using static ExampleService.Shared.Handlers;
using static ExampleService.Domain.Events;

namespace ExampleService.Domain
{
    public record MarketCurve
    {
        public IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();
        public string? Name { get; init; }

        public static CommandHandler<NameAndAddInstrument, MarketCurve> NameAndAdd = 
            (command, _) =>
            command.Name != null && command.Instrument != null
                ? new object[] { new MarketCurveNamed(command.Name), new InstrumentAddedToCurve(command.Instrument) }
                : Enumerable.Empty<object>();

        public static EventApplier<MarketCurveNamed, MarketCurve> MarketCurveNamed = (@event, state) =>
            state with { Name = @event.Name };

        public static EventApplier<InstrumentAddedToCurve, MarketCurve> InstrumentAdded = (@event, state) =>
            state with { Instruments = state.Instruments.Append(@event.InstrumentId).ToList() };
    }
}
