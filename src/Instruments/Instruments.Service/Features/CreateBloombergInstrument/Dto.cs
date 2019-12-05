using System;
using System.Collections.Generic;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateBloombergInstrument
{
    public class Dto
    {
        public IEnumerable<string> PricingSources { get; set; } = Enum.GetNames(typeof(PricingSource));
        public IEnumerable<string> YellowKeys { get; set; } = Enum.GetNames(typeof(YellowKey));

        public Command Command { get; set; } = new Command
        {
            Id = Guid.NewGuid(),
            PricingSource = PricingSource.CMPL.ToString(),
            YellowKey = YellowKey.INDEX.ToString(),
            Ticker = string.Empty
        };
    }
}
