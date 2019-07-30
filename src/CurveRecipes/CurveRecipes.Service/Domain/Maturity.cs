using System;
using Common.Core;

namespace CurveRecipes.Domain
{
    public class Maturity : ValueObject
    {
        public Maturity(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Must be => 0", nameof(value));
            }

            Value = value;
        }

        public double Value { get; }
    }
}
