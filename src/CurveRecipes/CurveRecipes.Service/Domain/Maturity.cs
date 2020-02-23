using Common.Core;
using LanguageExt;

namespace CurveRecipes.Domain
{
    public class Maturity : Record<Maturity>
    {
        public static Common.Core.Either<Error, Maturity> TryCreate(double value)
        {
            if (value < 0)
            {
                return new Error($"{nameof(value)} Must be => 0");
            }

            return new Maturity(value);
        }

        private Maturity(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}
