using System;
using System.Collections.Generic;
using System.Linq;
using Lib.EventSourcing;
using static ExampleService.WriteSide.MarketCurve.Events;

namespace ExampleService.ReadSide
{
    public class GetCurve : IQuery<GetCurve.Curve?>
    {
        public string? Id { get; init; }

        public class Projection : InMemoryProjection<Curve>
        {
            public Projection()
            {
                CreateOrUpdateWhen<MarketCurveNamed>((e, state) => state with { Id = e.AggregateId, Name = e.Content.Name });

                CreateOrUpdateWhen<InstrumentAddedToCurve>((e, state) => state with { Id = e.AggregateId, Instruments = state.Instruments.Append(e.Content.InstrumentId).ToList() });
            }

            protected override Curve InitializeModel() => new Curve(string.Empty, string.Empty, Array.Empty<string>());
        }

        public (long, Curve?) Handle() => InMemoryProjectionStore.Instance.Get<Curve>(Id ?? throw new Exception());

        public record Curve(string Id, string Name, IReadOnlyCollection<string> Instruments);
    }
}
