using LanguageExt;

namespace CalculationEngine.Domain
{
    public class Order : Record<Order>
    {
        public Order(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
