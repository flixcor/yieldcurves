using System;
using System.Collections.Generic;
using ExampleService.Domain;
using ExampleService.Shared;

namespace ExampleService.Features
{
    public record NameAndAdd : IQuery<IReadOnlyCollection<object>>
    {
        public string? Name { get; init; }
        public string? Instrument { get; init; }

        public IReadOnlyCollection<object> Handle() =>
            Name != null && Instrument != null
                ? MarketCurve.Name(Name).AddInstrument(Instrument).GetUncommittedEvents()
                : Array.Empty<object>();
    }
}
