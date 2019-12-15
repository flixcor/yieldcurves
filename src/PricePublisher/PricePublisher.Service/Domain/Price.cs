namespace PricePublisher.Service.Domain
{
    public class Price
    {
        public Price(Currency currency, double value)
        {
            Currency = currency;
            Value = value;
        }

        public Currency Currency { get; }
        public double Value { get; }
    }
}
