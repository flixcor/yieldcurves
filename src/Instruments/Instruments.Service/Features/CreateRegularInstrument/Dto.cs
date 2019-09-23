using System;
using System.Collections.Generic;
using Instruments.Domain;

namespace Instruments.Service.Features.CreateRegularInstrument
{
    public class Dto
    {
        public IEnumerable<string> Vendors { get; set; } = Enum.GetNames(typeof(Vendor));

        public Command Command { get; set; } = new Command
        {
            Id = Guid.NewGuid(),
            Vendor = Vendor.UBS,
            Name = string.Empty
        };
    }
}
