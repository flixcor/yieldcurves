using System;

namespace Common.Core
{
    public struct NonEmptyString : IEquatable<NonEmptyString>
    {
        private readonly string _value;

        internal NonEmptyString(string value)
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

    public static class NonEmptyStringExtensions
    {
        public static NonEmptyString NonEmpty(this string str) => new NonEmptyString(str);
        public static NonEmptyString NonEmptyString<T>(this T o) where T : Enum => new NonEmptyString(o.ToString());

    }

    public static class TNonEmpty
    {
        public static NonEmptyString NonEmptyString<T>(this T o) where T : class => new NonEmptyString(o.ToString() ?? nameof(T));
    }
}
