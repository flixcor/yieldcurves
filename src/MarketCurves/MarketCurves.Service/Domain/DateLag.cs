using System;

namespace MarketCurves.Domain
{
    public class DateLag
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
    }
}