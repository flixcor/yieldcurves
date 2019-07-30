using System;
using CalculationEngine.Domain;

namespace CalculationEngine.Service.Domain
{
    public class Date : IComparable<Date>
    {
        private readonly DateTime _dateTime;

        private Date(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public Date Ultimum(DateLag dateLag) => new Date(_dateTime.Date.AddDays(dateLag.Value));

        public static Date FromDateTime(DateTime dateTime) => new Date(dateTime);

        public int CompareTo(Date other) => (_dateTime.Date - other._dateTime.Date).Days;

        public override bool Equals(object obj) => obj is Date other && _dateTime.Date.Equals(other._dateTime.Date);

        public override int GetHashCode() => _dateTime.Date.GetHashCode();

        public override string ToString() => _dateTime.ToShortDateString();

        public static bool operator <=(Date a, Date b) => a._dateTime.Date <= b._dateTime.Date;
        public static bool operator >=(Date a, Date b) => a._dateTime.Date >= b._dateTime.Date;
    }
}
