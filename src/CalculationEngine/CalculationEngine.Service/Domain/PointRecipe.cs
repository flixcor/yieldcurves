﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalculationEngine.Service.Domain;
using Common.Core;

namespace CalculationEngine.Domain
{
    public class PointRecipe
    {
        public PointRecipe(Guid instrumentId, Tenor tenor, DateLag dateLag, PriceType? priceType = null)
        {
            InstrumentId = instrumentId;
            Tenor = tenor;
            DateLag = dateLag;
            PriceType = priceType;
        }

        public Guid InstrumentId { get; }
        public Tenor Tenor { get; }
        public DateLag DateLag { get; }
        public PriceType? PriceType { get; }

        public Either<Error, CurvePoint> GetPoint(IEnumerable<PublishedPricing> pricings, Date asOfDate)
        {
            var minDate = asOfDate.Ultimum(DateLag);
            var match = pricings
                .Where(x => x.InstrumentId == InstrumentId && x.AsOfDate >= minDate && x.AsOfDate <= asOfDate)
                .OrderByDescending(x => x.AsOfDate)
                .FirstOrDefault();

            if (match is null)
            {
                return new Error("Not found");
            }

            return new CurvePoint(Tenor.GetMaturity(), match.Price);
        }
    }
}
