using System;
using System.Collections.Generic;
using System.Linq;
using Lib.EventSourcing;
using static Lib.Domain.MarketCurve.Events;

namespace Lib.Features
{
    public class GetCurveList : IQuery<GetCurveList.CurveList>
    {
        public class Projection : InMemoryProjection<Curve>
        {
            public Projection()
            {
                CreateOrUpdateWhen<MarketCurveNamed>((e, c) => c with { Id = e.AggregateId, Name = e.Content.Name });
            }
        }

        public CurveList Handle() => new CurveList { Curves = InMemoryProjectionStore.Instance.GetAll<Curve>().Item2.ToList() };

        public record CurveList
        {
            public IReadOnlyCollection<Curve> Curves { get; init; } = new List<Curve>();
        }

        public record Curve
        {
            public string? Id { get; init; }
            public string? Name { get; init; }
        }
    }
}
