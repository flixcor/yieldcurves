using System;

namespace CalculationEngine.Domain
{
    public enum Interpolation
    {
        Linear,
        FixedForward
    }

    public static class InterpolationExtensions
    {
        public static CurvePoint Solve(this Interpolation interpolation, CurvePoint a, CurvePoint b, Maturity maturity)
        {
            if (a.Price.Currency != b.Price.Currency)
            {
                throw new NotImplementedException("Converting between currencies is not supported");
            }

            var pointA = a.ToPoint();
            var pointB = b.ToPoint();
            var x = maturity.ToX();

            var point = interpolation.SolveForY(pointA, pointB, x);

            return CurvePoint.FromPoint(point, a.Price.Currency);
        }

        private static Point SolveForY(this Interpolation interpolation, Point a, Point b, X x) => interpolation switch
        {
            Interpolation.Linear => LinearInterpolation.SolveForY(a, b, x),
            Interpolation.FixedForward => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };

    }
}
