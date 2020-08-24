﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExampleService.Shared;
using static ExampleService.Domain.MarketCurve.Events;

namespace ExampleService.Features
{
    public class GetCurve : IQuery<GetCurve.Curve>
    {
        public static Curve Project(Curve state, EventEnvelope eventWrapper) => eventWrapper.Content switch
        {
            MarketCurveNamed named => state with { Name = named.Name },
            InstrumentAddedToCurve instrument => state with { Instruments = state.Instruments.Append(instrument.InstrumentId).ToList() },
            _ => state
        };

        public Curve Handle(Curve input) => input;

        public record Curve
        {
            public string? Id { get; init; }
            public string? Name { get; init; }
            public IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();
        }
    }

    
}
