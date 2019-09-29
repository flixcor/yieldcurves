using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PricePublisher.Domain;

namespace PricePublisher.Service.Features.PublishPrice
{
    public class Dto
    {
        public Command Command { get; set; } = new Command();

        public ImmutableArray<string> PriceTypes { get; } = Enum.GetNames(typeof(PriceType)).ToImmutableArray();
        public IEnumerable<InstrumentDto> Instruments { get; set; } = new List<InstrumentDto>();
    }
}
