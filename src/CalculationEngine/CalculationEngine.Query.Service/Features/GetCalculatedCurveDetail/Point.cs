using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculatedCurveDetail
{
    public class Point : ReadObject
    {
        public double Maturity { get; set; }
        public string? Currency { get; set; }
        public double Value { get; set; }
    }
}
