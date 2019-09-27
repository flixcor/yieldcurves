using System;
using Common.Core;
using LanguageExt;

namespace CurveRecipes.Domain
{
    public class Maturity : Record<Maturity>
    {
        public static Result<Maturity> TryCreate(double value)
        {
            return value < 0 
                ? Result.Fail<Maturity>($"{nameof(value)} Must be => 0") 
                : Result.Ok(new Maturity(value));
        }

        private Maturity(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}
