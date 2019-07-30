using System;

namespace CalculationEngine.Service.ActorModel.Commands
{
    public class SendMeInstrumentPricingPublished
    {
        public SendMeInstrumentPricingPublished(Guid instrumentId)
        {
            InstrumentId = instrumentId;
        }

        public Guid InstrumentId { get; }
    }
}
