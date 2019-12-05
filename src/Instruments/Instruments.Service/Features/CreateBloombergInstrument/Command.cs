using System;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateBloombergInstrument
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public string PricingSource { get; set; }
        public string YellowKey { get; set; }
    }
}
