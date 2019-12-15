using System;
using Common.Core;
using Google.Protobuf;

namespace Common.Events
{
    public interface IRegularInstrumentCreated : IEvent
    {
        string Description { get; }
        string Vendor { get; }
    }

    internal partial class RegularInstrumentCreated : IRegularInstrumentCreated
    {
        public RegularInstrumentCreated(Guid aggregateId, string vendor, string description)
        {
            AggregateId = aggregateId.ToString();
            Vendor = vendor ?? throw new ArgumentNullException(nameof(vendor));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        Guid IEvent.AggregateId => Guid.Parse(AggregateId);

        public IEvent WithVersion(int version)
        {
            var clone = (RegularInstrumentCreated)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
