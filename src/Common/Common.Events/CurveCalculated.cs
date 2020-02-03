using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core;

namespace Common.Events
{
    public interface ICurveCalculated : IEvent
    {
        string AsOfDate { get; }
        Guid CurveRecipeId { get; }
        IEnumerable<IPoint> Points { get; }
    }

    internal partial class CurveCalculated : ICurveCalculated
    {
        public CurveCalculated(Guid curveRecipeId, string asOfDate, IEnumerable<IPoint> points)
        {
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;

            Points.Add(points?.Cast<Point>() ?? throw new ArgumentNullException(nameof(points)));
        }

        Guid ICurveCalculated.CurveRecipeId => CurveRecipeId;

        IEnumerable<IPoint> ICurveCalculated.Points => Points.Cast<IPoint>();
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
