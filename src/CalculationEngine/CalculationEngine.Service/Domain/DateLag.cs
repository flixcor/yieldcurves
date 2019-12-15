using LanguageExt;

namespace CalculationEngine.Domain
{
    public class DateLag : Record<DateLag>
    {
        public DateLag(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
