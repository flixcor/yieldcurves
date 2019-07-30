namespace MarketCurves.Domain
{
    public enum Vendor
    {
        Bloomberg,
        Ubs
    }

    public static class VendorExtensions
    {
        public static bool HasPriceType(this Vendor vendor)
        {
            return vendor switch
            {
                Vendor.Bloomberg => true,
                _ => false,
            };
        }
    }
}
