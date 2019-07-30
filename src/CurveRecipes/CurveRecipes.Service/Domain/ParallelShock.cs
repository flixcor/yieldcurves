using System;

namespace CurveRecipes.Domain
{
    public class ParallelShock : ITransformation
    {
        public ParallelShock(ShockTarget shockTarget, Shift shift)
        {
            ShockTarget = shockTarget;
            Shift = shift ?? throw new ArgumentNullException(nameof(shift));
        }

        public ShockTarget ShockTarget { get; }
        public Shift Shift { get; }
    }
}
