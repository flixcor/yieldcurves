using System;
using System.Collections.Generic;
using Common.Core;

namespace Common.Events
{
    public class CurveCalculated : Event
    {
        public CurveCalculated(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, IEnumerable<Point> points) : base(id)
        {
            CurveRecipeId = curveRecipeId;
            AsOfDate = asOfDate;
            AsAtDate = asAtDate;
            Points = points ?? throw new ArgumentNullException(nameof(points));
        }

        public Guid CurveRecipeId { get; }
        public DateTime AsOfDate { get; }
        public DateTime AsAtDate { get; }
        public IEnumerable<Point> Points { get; }

        public class Point
        {
            public Point(double maturity, string currency, double value)
            {
                Maturity = maturity;
                Currency = currency ?? throw new ArgumentNullException(nameof(currency));
                Value = value;
            }

            public double Maturity { get; }
            public string Currency { get; }
            public double Value { get; }
        }
    }
}
