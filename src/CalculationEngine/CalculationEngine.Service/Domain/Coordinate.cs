using Common.Core;

namespace CalculationEngine.Domain
{
    public class Point : ValueObject
    {
        public Point(X x, Y y)
        {
            X = x;
            Y = y;
        }

        public X X { get; }
        public Y Y { get; }
    }

    public abstract class Coordinate : ValueObject
    {
        protected Coordinate(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public static double operator /(Coordinate a, Coordinate b)
        {
            return a.Value / b.Value;
        }

        public static bool operator <(Coordinate a, Coordinate b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(Coordinate a, Coordinate b)
        {
            return a.Value > b.Value;
        }
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

        public static X operator /(X a, X b)
        {
            return new X(a.Value / b.Value);
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
    }
}
