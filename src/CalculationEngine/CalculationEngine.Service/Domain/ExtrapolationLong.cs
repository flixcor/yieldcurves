using System;

namespace CalculationEngine.Domain
{
    public enum ExtrapolationLong
    {
        FixedForward,
        Flat
    }

    public static class ExtrapolationLongExtensions
    {
        public static CurvePoint Solve(this ExtrapolationLong extraShort, CurvePoint min, Maturity maturity)
        {
            var p = min.ToPoint();
            var x = maturity.ToX();

            var point = extraShort.GetPoint(p, x);

            return CurvePoint.FromPoint(point, min.Price.Currency);
        }

        private static Point GetPoint(this ExtrapolationLong extrapolation, Point p, X x) => extrapolation switch
        {
            ExtrapolationLong.Flat => new Point(x, p.Y),
            ExtrapolationLong.FixedForward => new Point(x, p.Y),
            _ => throw new NotImplementedException(),
        };
    }
}
