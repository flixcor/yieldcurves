﻿using System;
using Common.Core;

namespace Common.Events
{
    public class CurvePointAdded : IEvent
    {
        public CurvePointAdded(Guid id, string tenor, Guid instrumentId, short dateLag, bool isMandatory, string priceType, int version = 0)
        {
            Id = id;
            Tenor = tenor;
            InstrumentId = instrumentId;
            DateLag = dateLag;
            IsMandatory = isMandatory;
            PriceType = priceType;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; }
        public string Tenor { get; }
        public Guid InstrumentId { get; }
        public short DateLag { get; }
        public bool IsMandatory { get; }
        public string PriceType { get; }

    }
}