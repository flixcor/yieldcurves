using System;

namespace Common.Core
{
    public struct NonEmptyGuid : IEquatable<NonEmptyGuid>
    {
        private readonly Guid _value;

        private NonEmptyGuid(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
        }

        public override bool Equals(object? obj) => obj is Guid guid && _value.Equals(guid);

        public bool Equals(NonEmptyGuid other) => _value.Equals(other._value);

        public override int GetHashCode() => HashCode.Combine(_value);

        public static bool operator ==(NonEmptyGuid left, NonEmptyGuid right) => left.Equals(right);

        public static bool operator !=(NonEmptyGuid left, NonEmptyGuid right) => !(left == right);

        public static implicit operator Guid(NonEmptyGuid nonEmpty) => nonEmpty._value;
        public static implicit operator NonEmptyGuid(Guid g) => new NonEmptyGuid(g);

        public override string? ToString() => _value.ToString();
        public string ToString(string format) => _value.ToString(format);
    }
}
