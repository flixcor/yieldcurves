using System;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateBloombergInstrument
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public PricingSource PricingSource { get; set; }
        public YellowKey YellowKey { get; set; }
    }
}
