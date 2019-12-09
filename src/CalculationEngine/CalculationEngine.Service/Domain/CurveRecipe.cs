using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CalculationEngine.Domain
{
    public class CurveRecipe
    {
        private readonly ImmutableArray<OrderedTransformation> _transformations;

        public CurveRecipe(Guid id, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation,
            ExtrapolationShort extrapolationShort, ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType, params OrderedTransformation[] transformations)
        {
            Id = id;
            LastLiquidTenor = lastLiquidTenor;
            DayCountConvention = dayCountConvention;
            Interpolation = interpolation;
            ExtrapolationShort = extrapolationShort;
            ExtrapolationLong = extrapolationLong;
            OutputFrequency = outputFrequency;
            OutputType = outputType;
            _transformations = transformations?.ToImmutableArray() ?? ImmutableArray<OrderedTransformation>.Empty;
        }

        public Guid Id { get; }
        public Tenor LastLiquidTenor { get; }
        public DayCountConvention DayCountConvention { get; }
        public Interpolation Interpolation { get; }
        public ExtrapolationShort ExtrapolationShort { get; }
        public ExtrapolationLong ExtrapolationLong { get; }
        public OutputFrequency OutputFrequency { get; }
        public OutputType OutputType { get; }

        public IEnumerable<CurvePoint> ApplyTo(IEnumerable<CurvePoint> points) => 
            _transformations
                .OrderBy(x => x.Order)
                .Select(x => x.Transformation)
                .Aggregate(ResolveAllMaturities(points), (p, t) => t.Transform(p));

        private IEnumerable<CurvePoint> ResolveAllMaturities(IEnumerable<CurvePoint> points) => 
            OutputFrequency.GetMaturities()
                .Select(maturity => ResolveMaturity(maturity, points));

        private CurvePoint ResolveMaturity(Maturity maturity, IEnumerable<CurvePoint> points)
        {
            var alreadyExistingPoint = points.FirstOrDefault(p => p.Maturity == maturity);

                if (alreadyExistingPoint != null)
                {
                    return alreadyExistingPoint;
                }

                var minPoint = points
                    .OrderBy(p => p.Maturity)
                    .First();

                if (maturity < minPoint.Maturity)
                {
                    return ExtrapolationShort.Solve(minPoint, maturity);
                }

                var maxPoint = points
                    .OrderBy(p => p.Maturity)
                    .Last();

                if (maturity > maxPoint.Maturity)
                {
                    return ExtrapolationLong.Solve(maxPoint, maturity);
                }

                var before = points
                    .Where(p => p.Maturity < maturity)
                    .OrderBy(p => p.Maturity)
                    .Last();

                var after = points
                    .Where(p => p.Maturity > maturity)
                    .OrderBy(p => p.Maturity)
                    .First();

                return Interpolation.Solve(before, after, maturity);
        }
    }
}
