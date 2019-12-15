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
        public InstrumentCreated(Guid aggregateId, string vendor, string description, bool hasPriceType = false)
        {
            AggregateId = aggregateId.ToString();
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasPriceType = hasPriceType;
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (InstrumentCreated)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
