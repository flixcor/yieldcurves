using System;
using Common.Core;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateRegularInstrument
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Vendor { get; set; }
    }
}
