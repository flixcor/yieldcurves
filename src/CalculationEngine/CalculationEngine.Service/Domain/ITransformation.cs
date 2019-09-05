

using System.Collections.Generic;
using System.Collections.Immutable;

namespace CalculationEngine.Domain
{
    public interface ITransformation
    {
        ImmutableArray<CurvePoint> Transform(ICollection<CurvePoint> points);
    }
}
