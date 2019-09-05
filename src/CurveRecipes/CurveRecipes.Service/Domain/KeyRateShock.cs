using System;
using System.Collections.Generic;
using System.Linq;

namespace CurveRecipes.Domain
{
    public class KeyRateShock : ITransformation
    {
        public KeyRateShock(ShockTarget shockTarget, Shift shift, Maturity[] maturities)
        {
            ShockTarget = shockTarget;
            Shift = shift ?? throw new ArgumentNullException(nameof(shift));

            if (maturities == null)
            {
                throw new ArgumentNullException(nameof(maturities));
            }
            if (!maturities.Any())
            {
                throw new ArgumentException("must be more than 0", nameof(maturities));
            }

            Maturities = maturities;
        }

        public ShockTarget ShockTarget { get; }
        public Shift Shift { get; }
        public ICollection<Maturity> Maturities { get; }
    }
}
