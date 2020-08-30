using System.Collections.Generic;
using System.Linq;
using Lib.EventSourcing;
using static Lib.Domain.MarketCurve.Events;

namespace Lib.Features
{
    public class GetCurve : IQuery<GetCurve.Curve>
    {
        public string Id { get; init; }

        public class Projection : InMemoryProjection<Curve>
        {
            public Projection()
            {
                CreateOrUpdateWhen<MarketCurveNamed>((e, state) => state with { Id = e.AggregateId, Name = e.Content.Name });

                CreateOrUpdateWhen<InstrumentAddedToCurve>((e, state) => state with { Id = e.AggregateId, Instruments = state.Instruments.Append(e.Content.InstrumentId).ToList() });
            }
        }

        public (long, Curve) Handle() => InMemoryProjectionStore.Instance.Get<Curve>(Id);

        public record Curve
        {
            public string? Id { get; init; }
            public string? Name { get; init; }
            public IReadOnlyCollection<string> Instruments { get; init; } = new List<string>();
        }
    }
}
