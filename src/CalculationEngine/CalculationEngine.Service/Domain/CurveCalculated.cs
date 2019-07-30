using System;
using System.Collections.Generic;
using Common.Core;
using Newtonsoft.Json;

namespace CalculationEngine.Domain
{
    public class CurveCalculated : Event
    {
        [JsonConstructor]
        private CurveCalculated(Guid id, Guid curveRecipeId, DateTime asOfDate, DateTime asAtDate, IEnumerable<Point> points, int version) : this(id, curveRecipeId, asOfDate, asAtDate, points)
        {
            Version = version;
        }

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
