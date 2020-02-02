using System;
using Common.Core;

namespace Common.Events
{
    public interface IRegularInstrumentCreated : IEvent
    {
        string Description { get; }
        string Vendor { get; }
    }

    internal partial class RegularInstrumentCreated : IRegularInstrumentCreated
    {
        public RegularInstrumentCreated(string vendor, string description)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
