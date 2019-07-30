namespace CalculationEngine.Domain
{
    public static class LinearInterpolation
    {
        public static Point SolveForY(Point a, Point b, X x)
        {
            var min = a.X.Value < b.X.Value
                ? a
                : b;

            var max = a.X.Value > b.X.Value
                ? a
                : b;

            var deltaXAB = (max.X - min.X);
            var deltaYAB = (max.Y - min.Y);

            var ratio = deltaYAB / deltaXAB;

            var delta = x - min.X;

            var value = delta.Value * ratio;

            return new Point(x, new Y(value));
        }
    }
}
