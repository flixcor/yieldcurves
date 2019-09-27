namespace CalculationEngine.Domain
{
    public static class LinearInterpolation
    {
        public static Point SolveForY(Point a, Point b, X x)
        {
            var min = a.X < b.X
                ? a
                : b;

            var max = a.X > b.X
                ? a
                : b;

            var deltaX = (max.X - min.X);
            var deltaY = (max.Y - min.Y);

            var ratio = deltaY / deltaX;

            var delta = x - min.X;

            var newCoordinate = delta * ratio;

            return new Point(x, newCoordinate.ToY());
        }
    }
}
