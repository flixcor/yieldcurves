using System;
using System.Globalization;
using Common.Core;

namespace PricePublisher.Service.Domain
{
    public class Date
    {
        private const string Format = "yyyy-MM-dd";
        private readonly string _dateString;

        public static Either<Error, Date> TryParse(string dateString)
        {
            if (!DateTime.TryParseExact(dateString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return new Error($"value {dateString} does not comply to format {Format}");
            }

            return new Date(dateString);
        }

        public override string ToString() => _dateString;

        private Date(string dateString)
        {
            _dateString = dateString;
        }
    }
}
