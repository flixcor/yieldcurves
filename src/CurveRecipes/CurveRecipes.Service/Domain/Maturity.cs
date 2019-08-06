using System;
using Common.Core;

namespace CurveRecipes.Domain
{
    public class Maturity : ValueObject
    {
        public static Result<Maturity> TryCreate(double value)
        {
            if (value < 0)
            {
                return Result.Fail<Maturity>($"{nameof(value)} Must be => 0");
            }

            return Result.Ok(new Maturity(value));
        }

        private Maturity(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}
