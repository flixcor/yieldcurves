using System;
using CalculationEngine.Domain;
using LanguageExt;

namespace CalculationEngine.Service.Domain
{
    public class Date : Record<Date>
    {
        private readonly DateTime _dateTime;

        private Date(DateTime dateTime)
        {
            _dateTime = dateTime.Date;
        }

        public Date Ultimum(DateLag dateLag) => new Date(_dateTime.Date.AddDays(dateLag.Value));

        public static Date FromDateTime(DateTime dateTime) => new Date(dateTime);

        public override string ToString() => _dateTime.ToShortDateString();
    }
}
