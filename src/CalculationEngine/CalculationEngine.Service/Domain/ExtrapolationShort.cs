using System;

namespace CalculationEngine.Domain
{
    public enum ExtrapolationShort
    {
        Zero,
        Flat
    }

    public static class ExtrapolationShortExtensions
    {
        public static CurvePoint Solve(this ExtrapolationShort extraShort, CurvePoint min, Maturity maturity)
        {
            var p = min.ToPoint();
            var x = maturity.ToX();

            var point = extraShort.GetPoint(p, x);

            return CurvePoint.FromPoint(point, min.Price.Currency);
        }

        private static Point GetPoint(this ExtrapolationShort extrapolation, Point p, X x) => extrapolation switch
        {
            ExtrapolationShort.Zero => new Point(x, new Y(0)),
            ExtrapolationShort.Flat => new Point(x, p.Y),
            _ => throw new NotImplementedException()
        };
    }
}
