

using System.Collections.Generic;
using System.Collections.Immutable;

namespace CalculationEngine.Domain
{
    public interface ITransformation
    {
        IEnumerable<CurvePoint> Transform(IEnumerable<CurvePoint> points);
    }
}
