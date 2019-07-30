using Common.Core;

namespace PricePublisher.Domain
{
    public class Currency
    {
        private readonly string _value;

        private Currency(string value)
        {
            _value = value;
        }

        public static Result<Currency> FromString(string input)
        {
            return string.IsNullOrWhiteSpace(input) || input.Length != 3
                ? Result.Fail<Currency>("currency cannot be empty and must be 3 characters")
                : Result.Ok(new Currency(input));
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
