using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Core;
using MarketCurves.Domain;

namespace MarketCurves.Service.Domain
{
    public class Instrument : IEquatable<Instrument?>
    {
        public delegate Task<Either<Error,Vendor>> GetVendor(NonEmptyGuid id);

        private Instrument(NonEmptyGuid id)
        {
            Id = id;
        }

        public NonEmptyGuid Id { get; }
        public Vendor Vendor { get; private set; }

        public static async Task<Either<Error,Instrument>> FromId(NonEmptyGuid id, GetVendor getVendor)
        {
            var vendor = await getVendor(id);

            return vendor.MapRight(v => new Instrument(id) { Vendor = v });
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Instrument);
        }

        public bool Equals(Instrument? other)
        {
            return other is Instrument i &&
                   Id.Equals(i.Id) &&
                   Vendor == i.Vendor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Vendor);
        }

        public static bool operator ==(Instrument left, Instrument right)
        {
            return EqualityComparer<Instrument>.Default.Equals(left, right);
        }

        public static bool operator !=(Instrument left, Instrument right)
        {
            return !(left == right);
        }
    }
}
