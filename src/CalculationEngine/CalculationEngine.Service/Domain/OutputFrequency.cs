using System;
using System.Collections.Generic;
using LanguageExt;

namespace CalculationEngine.Domain
{
    public class OutputFrequency : Record<OutputFrequency>
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
            return OutputSeries switch
            {
                OutputSeries.SemiAnnual => GetSemiAnnualMaturities(),
                OutputSeries.Monthly => GetMonthlyMaturities(),
                _ => GetAnnualMaturities(),
            };
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
            for (double i = 0; i <= MaximumMaturity.Value; i += 1D / 12)
            {
                yield return new Maturity(i);
            }
        }
    }
}
