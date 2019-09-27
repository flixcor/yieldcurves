using System;
using LanguageExt;

namespace CalculationEngine.Domain
{
    public class CurvePoint : Record<CurvePoint>
    {
        public CurvePoint(Maturity maturity, Price price)
        {
            Maturity = maturity;
            Price = price;
        }

        public Maturity Maturity { get; }
        public Price Price { get; }

        public Point ToPoint() => new Point(Maturity.ToX(), Price.ToY());

        public static CurvePoint FromPoint(Point point, string currency) =>
            new CurvePoint(Maturity.FromX(point.X), Price.FromY(point.Y, currency));
    }
}
