using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features
{
    public class GetCommands : IQuery<GetCommandsDto>
    {
        public class Handler : IHandleQuery<GetCommands, GetCommandsDto>
        {
            public Task<GetCommandsDto> Handle(GetCommands query, CancellationToken cancellationToken)
            {
                return Task.FromResult(new GetCommandsDto());
            }
        }
    }

    public class GetCommandsDto
    {
        public GetCreateBloombergInstrumentDto Bloomberg { get; set; } = new GetCreateBloombergInstrumentDto();
        public GetCreateRegularInstrumentDto Regular { get; set; } = new GetCreateRegularInstrumentDto();
    }

    public class GetCreateRegularInstrumentDto
    {
        public IEnumerable<string> Vendors { get; set; } = Enum.GetNames(typeof(Vendor));

        public CreateRegularInstrument Command { get; set; } = new CreateRegularInstrument
        {
            Id = Guid.NewGuid(),
            Vendor = Vendor.UBS,
            Name = string.Empty
        };
    }

    public class GetCreateBloombergInstrumentDto
    {
        public IEnumerable<string> PricingSources { get; set; } = Enum.GetNames(typeof(PricingSource));
        public IEnumerable<string> YellowKeys { get; set; } = Enum.GetNames(typeof(YellowKey));

        public CreateBloombergInstrument Command { get; set; } = new CreateBloombergInstrument
        {
            Id = Guid.NewGuid(),
            PricingSource = PricingSource.CMPL,
            YellowKey = YellowKey.INDEX,
            Ticker = string.Empty
        };
    }
}
