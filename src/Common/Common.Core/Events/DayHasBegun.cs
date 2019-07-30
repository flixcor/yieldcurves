using System;

namespace Common.Core.Events
{
    public class DayHasBegun : Event
    {
        public DayHasBegun(Guid id, DateTime date, int version) : this(id, date)
        {
            Version = version;
        }

        public DayHasBegun(Guid id, DateTime date) : base(id)
        {
            Date = date;
        }

        public DateTime Date { get; }
    }
}
