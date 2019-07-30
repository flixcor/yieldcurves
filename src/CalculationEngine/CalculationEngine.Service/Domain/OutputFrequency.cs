using System;
using System.Collections.Generic;

namespace CalculationEngine.Domain
{
    public class OutputFrequency
    {
        public OutputFrequency(OutputSeries outputSeries, Maturity maximumMaturity)
        {
            OutputSeries = outputSeries;
            MaximumMaturity = maximumMaturity;
        }

        public OutputSeries OutputSeries { get; private set; }
        public Maturity MaximumMaturity { get; private set; }

        public IEnumerable<Maturity> GetMaturities()
        {
            switch (OutputSeries)
            {
                default:
                case OutputSeries.Annual:
                    return GetAnnualMaturities();
                case OutputSeries.SemiAnnual:
                    return GetSemiAnnualMaturities();
                case OutputSeries.Monthly:
                    return GetMonthlyMaturities();
            }
        }

        private IEnumerable<Maturity> GetAnnualMaturities()
        {
            var intMat = Math.Floor(MaximumMaturity.Value);

            for (var i = 0; i <= intMat; i++)
            {
                yield return new Maturity(i);
            }
        }

        private IEnumerable<Maturity> GetSemiAnnualMaturities()
        {
            for (double i = 0; i <= MaximumMaturity.Value; i+=0.5)
            {
                yield return new Maturity(i);
            }
        }

        private IEnumerable<Maturity> GetMonthlyMaturities()
        {
            for (double i = 0; i <= MaximumMaturity.Value; i += 1/12)
            {
                yield return new Maturity(i);
            }
        }
    }
}
