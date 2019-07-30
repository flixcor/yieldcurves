namespace CurveRecipes.Domain
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
    }
}
