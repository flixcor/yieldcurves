using System;
using System.Collections.Generic;
using System.Linq;
using ExampleService.Domain;
using ExampleService.Shared;
using static ExampleService.Domain.Events;

namespace ExampleService.Features
{
    public class GetCurveList : IQuery<CurveList>
    {
        public static CurveList Project(CurveList state, EventEnvelope eventWrapper) => eventWrapper.Content switch
        {
            MarketCurveNamed named => AddOrUpdate(state, eventWrapper.AggregateId, (c) => c with { Name = named.Name }),
            InstrumentAddedToCurve instrument => AddOrUpdate(state, eventWrapper.AggregateId, (c) => c with { Instruments = c.Instruments.Append(instrument.InstrumentId).ToList() }),
            _ => state
        };

        private static CurveList AddOrUpdate(CurveList state, string id, Func<Curve, Curve> func)
        {
            var inputCurve = state.Curves.FirstOrDefault(x => x.Id == id) ?? new Curve { Id = id };
            var outputCurve = func(inputCurve);
            return state with { Curves = state.Curves.Where(x => x.Id != id).Append(outputCurve).ToList() };
        }

        public CurveList Handle(CurveList input) => input;
    }

    public record CurveList
    {
        public IReadOnlyCollection<Curve> Curves { get; init; } = new List<Curve>();
    }

    public record Curve
    {
        public string? Id { get; init; }
        public string? Name { get; init; }
        public IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();
    }
}
