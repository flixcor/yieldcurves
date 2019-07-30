using Common.Core;

namespace CalculationEngine.Domain
{
    public class Price : ValueObject
    {
        public Price(string currency, double value)
        {
            Currency = currency;
            Value = value;
        }

        public string Currency { get; }
        public double Value { get; }

        public Y ToY()
        {
            return new Y(Value);
        }

        public static Price FromY(Y y, string currency)
        {
            return new Price(currency, y.Value);
        }
    }
}
