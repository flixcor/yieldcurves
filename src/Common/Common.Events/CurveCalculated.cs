using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;
using Google.Protobuf.WellKnownTypes;

namespace Common.Events
{
    public interface ICurveCalculated : IEvent
    {
        DateTime AsAtDate { get; }
        string AsOfDate { get; }
        Guid CurveRecipeId { get; }
        IEnumerable<IPoint> Points { get; }
    }

    internal partial class CurveCalculated : ICurveCalculated
    {
        public CurveCalculated(Guid aggregateId, Guid curveRecipeId, string asOfDate, DateTime asAtDate, IEnumerable<IPoint> points)
        {
            AggregateId = aggregateId.ToString("N");
            CurveRecipeId = curveRecipeId.ToString();
            AsOfDate = asOfDate;
            AsAtDate = Timestamp.FromDateTime(asAtDate.ToUniversalTime());

            Points.Add(points?.Cast<Point>() ?? throw new ArgumentNullException(nameof(points)));
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        DateTime ICurveCalculated.AsAtDate => AsAtDate.ToDateTime();

        Guid ICurveCalculated.CurveRecipeId => Guid.Parse(CurveRecipeId);

        IEnumerable<IPoint> ICurveCalculated.Points => Points.Cast<IPoint>();

        public IEvent WithVersion(int version)
        {
            var clone = (CurveCalculated)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }

    internal partial class Point : IPoint
    {
        public Point(double maturity, string currency, double value)
        {
            Maturity = maturity;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Value = value;
        }
    }

    public interface IPoint
    {
        string Currency { get; set; }
        double Maturity { get; set; }
        double Value { get; set; }
    }
}
