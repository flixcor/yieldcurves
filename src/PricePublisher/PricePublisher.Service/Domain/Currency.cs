using Common.Core;

namespace PricePublisher.Service.Domain
{
    public class Currency
    {
        private readonly string _value;

        private Currency(string value)
        {
            _value = value;
        }

        public static Either<Error,Currency> FromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 3)
            {
                return new Error("currency cannot be empty and must be 3 characters");
            }

            return new Currency(input);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
