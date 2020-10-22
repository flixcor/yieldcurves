using System.Collections.Generic;
using System.Linq;
using Lib.EventSourcing;
using static ExampleService.WriteSide.MarketCurve.Events;

namespace ExampleService.ReadSide
{
    public class GetCurveList : IQuery<GetCurveList.CurveList>
    {
        public class Projection : InMemoryProjection<Curve>
        {
            public Projection()
            {
                CreateOrUpdateWhen<MarketCurveNamed>((e, c) => c with { Id = e.AggregateId, Name = e.Content.Name });
            }

            protected override Curve InitializeModel() => new Curve(string.Empty, string.Empty);
        }

        public (long, CurveList) Handle()
        {
            var (position, curves) = InMemoryProjectionStore.Instance.GetAll<Curve>();
            return (position, new CurveList { Curves = curves.ToList() });
        }

        public record CurveList
        {
            public IReadOnlyCollection<Curve> Curves { get; init; } = new List<Curve>();
        }

        public record Curve(string Id, string Name);
    }
}
