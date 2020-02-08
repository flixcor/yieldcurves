using System;
using System.Collections.Generic;

namespace MarketCurves.Domain
{
    public class DateLag : IEquatable<DateLag>, IComparable<DateLag>
    {
        public static readonly DateLag None = new DateLag(0);

        public DateLag(short value)
        {
            if (value > 0)
            {
                throw new ArgumentException("should be <= 0", nameof(value));
            }
        }

        private DateLag()
        {
        }

        public short Value { get; private set; }

        public int CompareTo(DateLag? other)
        {
            return other is null
                ? 1
                : Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is DateLag dl && Equals(dl);
        }

        public bool Equals(DateLag other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(DateLag left, DateLag right)
        {
            return EqualityComparer<DateLag>.Default.Equals(left, right);
        }

        public static bool operator !=(DateLag left, DateLag right)
        {
            return !(left == right);
        }
    }
}
