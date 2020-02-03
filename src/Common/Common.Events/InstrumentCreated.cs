using System;
using Common.Core;

namespace Common.Events
{
    public interface IInstrumentCreated : IEvent
    {
        string Description { get; }
        bool HasPriceType { get; }
        string Vendor { get; }
    }

    internal partial class InstrumentCreated : IInstrumentCreated
    {
        public InstrumentCreated(string vendor, string description, bool hasPriceType)
        {
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
        }
    }
}
