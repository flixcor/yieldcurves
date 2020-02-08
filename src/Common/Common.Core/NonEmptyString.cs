using System;

namespace Common.Core
{
    public struct NonEmptyString : IEquatable<NonEmptyString>
    {
        private readonly string _value;

        public NonEmptyString(string value)
        {
            if (value == string.Empty)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
        }

        public override bool Equals(object? obj) => obj is NonEmptyString @string && Equals(@string);

        public bool Equals(NonEmptyString other) => _value == other._value;

        public override int GetHashCode() => HashCode.Combine(_value);

        public override string? ToString() => _value;

        public static bool operator ==(NonEmptyString left, NonEmptyString right) => left.Equals(right);

        public static bool operator !=(NonEmptyString left, NonEmptyString right) => !(left == right);

        public static implicit operator string(NonEmptyString nonEmpty) => nonEmpty._value;
    }
}
