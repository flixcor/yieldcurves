using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instruments.Service.Features.GetCommands
{
    public class Dto
    {
        public CreateBloombergInstrument.Dto Bloomberg { get; set; } = new CreateBloombergInstrument.Dto();
        public CreateRegularInstrument.Dto Regular { get; set; } = new CreateRegularInstrument.Dto();
    }
}
