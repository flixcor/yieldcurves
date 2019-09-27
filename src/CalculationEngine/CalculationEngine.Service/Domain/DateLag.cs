using LanguageExt;

namespace CalculationEngine.Domain
{
    public class DateLag : Record<DateLag>
    {
        public DateLag(short value)
        {
            Value = value;
        }

        public short Value { get; }
    }
}
