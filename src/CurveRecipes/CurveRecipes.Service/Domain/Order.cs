using LanguageExt;

namespace CurveRecipes.Domain
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
