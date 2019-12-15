using System;
using System.Globalization;
using CalculationEngine.Domain;
using LanguageExt;

namespace CalculationEngine.Service.Domain
{
    public class Date : Record<Date>
    {
        private const string Format = "yyyy-MM-dd";
        private readonly DateTime _dateTime;

        private Date(DateTime dateTime)
        {
            _dateTime = dateTime.Date;
        }

        public Date Ultimum(DateLag dateLag) => AddDays(dateLag.Value);

        public Date AddDays(int days) => FromDateTime(_dateTime.AddDays(days));

        public static Date FromDateTime(DateTime dateTime) => new Date(dateTime);
        public static Date FromString(string dateString) => new Date(DateTime.ParseExact(dateString, Format, CultureInfo.InvariantCulture));

        public DateTime ToDateTime() => _dateTime;

        public override string ToString() => _dateTime.ToString(Format);
    }
}
