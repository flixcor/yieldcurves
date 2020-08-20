using System.Collections.Generic;
using System.Linq;
using ExampleService.Shared;

namespace ExampleService.Domain
{
    public record MarketCurveState : EsAggregateState<MarketCurveState>
    {
        private IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();

        public string? Name { get; init; }

        protected override MarketCurveState When(object @event) => @event switch
        {
            MarketCurveNamed named => this with { Name = named.Name },
            InstrumentAdded instrumentAdded => this with { Instruments = Instruments.Append(instrumentAdded.InstrumentId).ToList() },
            _ => this
        };
    }
}
