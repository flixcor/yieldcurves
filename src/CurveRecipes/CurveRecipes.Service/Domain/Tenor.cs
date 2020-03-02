using Common.Core;

namespace CurveRecipes.Domain
{
    public enum Tenor
    {
        ON,
        D2,
        W1,
        W2,
        W3,
        M1,
        M2,
        M3,
        M4,
        M5,
        M6,
        M7,
        M8,
        M9,
        M10,
        M11,

        FRA1x7,
        FRA2x8,
        FRA3x9,
        FRA4x10,
        FRA5x11,
        FRA6x12,
        FRA7x13,
        FRA8x14,
        FRA9x15,
        FRA10x16,
        FRA11x17,
        FRA12x18,

        Y01,
        M18,
        Y02,
        Y03,
        Y04,
        Y05,
        Y06,
        Y07,
        Y08,
        Y09,
        Y10,
        Y11,
        Y12,
        Y13,
        Y14,
        Y15,
        Y16,
        Y17,
        Y18,
        Y19,
        Y20,
        Y21,
        Y22,
        Y23,
        Y24,
        Y25,
        Y26,
        Y27,
        Y28,
        Y29,
        Y30,
        Y31,
        Y32,
        Y33,
        Y34,
        Y35,
        Y36,
        Y37,
        Y38,
        Y39,
        Y40,
        Y41,
        Y42,
        Y43,
        Y44,
        Y45,
        Y46,
        Y47,
        Y48,
        Y49,
        Y50,
        Y51,
        Y52,
        Y53,
        Y54,
        Y55,
        Y56,
        Y57,
        Y58,
        Y59,
        Y60
    }

    public static class TenorExtensions
    {
        public static Either<Error, Maturity> GetMaturity(this Tenor tenor)
        {
            var multiplier = (tenor.ToString()[0]) switch
            {
                'M' => 1D / 12,
                _ => 1,
            };
            var right = int.Parse(tenor.ToString().Substring(1));

            return Maturity.TryCreate(right * multiplier);
        }
    }
}
