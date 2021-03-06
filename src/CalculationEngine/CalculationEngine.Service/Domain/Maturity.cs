﻿using System;
using LanguageExt;

namespace CalculationEngine.Domain
{
    public class Maturity : Record<Maturity>
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

        public X ToX()
        {
            return new X(Value);
        }

        public static Maturity FromX(X x)
        {
            return new Maturity(x.Value);
        }

        public static bool operator <(Maturity a, Maturity b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(Maturity a, Maturity b)
        {
            return a.Value > b.Value;
        }

        public static bool operator >=(Maturity a, Maturity b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Maturity a, Maturity b)
        {
            return a.Value <= b.Value;
        }
    }
}
