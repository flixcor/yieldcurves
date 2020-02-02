using Common.Core;

namespace Common.Events
{
    public interface IParallelShockAdded : IEvent
    {
        int Order { get; }
        double Shift { get; }
        string ShockTarget { get; }
    }

    internal partial class ParallelShockAdded : IParallelShockAdded
    {
        public ParallelShockAdded(int order, string shockTarget, double shift)
        {
            Order = order;
            ShockTarget = shockTarget;
            Shift = shift;
        }
    }
}
