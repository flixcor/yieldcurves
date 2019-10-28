using LanguageExt;

namespace CalculationEngine.Domain
{
    public class Point : Record<Point>
    {
        public Point(X x, Y y)
        {
            X = x;
            Y = y;
        }

        public X X { get; }
        public Y Y { get; }
    }

    public class X : Coordinate
    {
        public X(double value) : base(value)
        {
        }

        public static X operator -(X a, X b)
        {
            return new X(a.Value - b.Value);
        }

        public static X operator +(X a, X b)
        {
            return new X(a.Value + b.Value);
        }

        public static X operator /(X a, X b)
        {
            return new X(a.Value / b.Value);
        }

        public static X operator *(X a, X b)
        {
            return new X(a.Value * b.Value);
        }
    }

    public class Coordinate : Record<Coordinate>
    {
        protected Coordinate(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public static Coordinate operator -(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.Value - b.Value);
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.Value + b.Value);
        }

        public static Coordinate operator /(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.Value / b.Value);
        }

        public static Coordinate operator *(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.Value * b.Value);
        }
    }

    public class Y : Coordinate
    {
        public Y(double value) : base(value)
        {
        }

        public static Y operator -(Y a, Y b)
        {
            return new Y(a.Value - b.Value);
        }

        public static Y operator /(Y a, Y b)
        {
            return new Y(a.Value / b.Value);
        }

        public static Y operator *(Y a, Y b)
        {
            return new Y(a.Value * b.Value);
        }
    }

    public static class CoordinateExtensions
    {
        public static Y ToY(this Coordinate coordinate) => new Y(coordinate.Value);
        public static X ToX(this Coordinate coordinate) => new X(coordinate.Value);
    }
}
