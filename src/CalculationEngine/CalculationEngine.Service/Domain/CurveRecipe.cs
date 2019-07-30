using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationEngine.Domain
{
    public class CurveRecipe
    {
        private readonly List<OrderedTransformation> _transformations;

        public CurveRecipe(Guid id, Tenor lastLiquidTenor, DayCountConvention dayCountConvention, Interpolation interpolation,
            ExtrapolationShort extrapolationShort, ExtrapolationLong extrapolationLong, OutputFrequency outputFrequency, OutputType outputType, List<OrderedTransformation> transformations = null)
        {
            Id = id;
            LastLiquidTenor = lastLiquidTenor;
            DayCountConvention = dayCountConvention;
            Interpolation = interpolation;
            ExtrapolationShort = extrapolationShort;
            ExtrapolationLong = extrapolationLong;
            OutputFrequency = outputFrequency;
            OutputType = outputType;
            _transformations = transformations ?? new List<OrderedTransformation>();
        }

        public Guid Id { get; }
        public Tenor LastLiquidTenor { get; }
        public DayCountConvention DayCountConvention { get; }
        public Interpolation Interpolation { get; }
        public ExtrapolationShort ExtrapolationShort { get; }
        public ExtrapolationLong ExtrapolationLong { get; }
        public OutputFrequency OutputFrequency { get; }
        public OutputType OutputType { get; }
        public IReadOnlyList<OrderedTransformation> Transformations => _transformations.AsReadOnly();

        public IEnumerable<CurvePoint> ApplyTo(IEnumerable<CurvePoint> points)
        {
            var allPoints = ResolveAllMaturities(points);

            foreach (var transformation in _transformations.OrderBy(x => x.Order).Select(x => x.Transformation))
            {
                //allPoints = transformation.Transform(allPoints.AsEnumerable());
            }

            return allPoints.ToList();
        }

        private IEnumerable<CurvePoint> ResolveAllMaturities(IEnumerable<CurvePoint> points)
        {
            foreach (var maturity in OutputFrequency.GetMaturities())
            {
                var alreadyExistingPoint = points.FirstOrDefault(p => p.Maturity == maturity);

                if (alreadyExistingPoint != null)
                {
                    yield return alreadyExistingPoint;
                    continue;
                }

                var minPoint = points.OrderBy(p => p.Maturity).First();

                if (maturity < minPoint.Maturity)
                {
                    yield return ExtrapolationShort.Solve(minPoint, maturity);
                    continue;
                }

                var maxPoint = points.OrderBy(p => p.Maturity).Last();

                if (maturity > maxPoint.Maturity)
                {
                    yield return ExtrapolationLong.Solve(maxPoint, maturity);
                    continue;
                }

                var before = points.Where(p => p.Maturity < maturity).OrderBy(p => p.Maturity).Last();
                var after = points.Where(p => p.Maturity > maturity).OrderBy(p => p.Maturity).First();

                yield return Interpolation.Solve(before, after, maturity);
            }
        }
    }
}
