using Common.Core;
using Newtonsoft.Json;
using System;

namespace Common.Core.Events
{
    public class CurvePointAdded : Event
    {
        public string Tenor { get; }
        public Guid InstrumentId { get; }
        public short DateLag { get; }
        public bool IsMandatory { get; }
        public string PriceType { get; }

        public CurvePointAdded(Guid id, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType) : base(id)
        {
            Tenor = tenor;
            InstrumentId = instrumentId;
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
        }

        [JsonConstructor]
        protected CurvePointAdded(Guid id, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType, int version) : this(id, tenor, instrumentId, dateLag, isMandatory, priceType)
        {
            Version = version;
        }
    }
}
