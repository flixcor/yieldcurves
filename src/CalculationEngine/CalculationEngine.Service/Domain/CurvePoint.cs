using System;
using Common.Core;

namespace CalculationEngine.Domain
{
    public class CurvePoint : ValueObject, IComparable<CurvePoint>
    {
        public CurvePoint(Maturity maturity, Price price)
        {
            Maturity = maturity;
            Price = price;
        }

        public Maturity Maturity { get; }
        public Price Price { get; }

        public Point ToPoint()
        {
            return new Point(Maturity.ToX(), Price.ToY());
        }

        public static CurvePoint FromPoint(Point point, string currency)
        {
            return new CurvePoint(Maturity.FromX(point.X), Price.FromY(point.Y, currency));
        }

        public int CompareTo(CurvePoint other)
        {
            return Maturity.CompareTo(other.Maturity);
        }
    }
}
