using System;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculatedCurveDetail
{
    public class Query : IQuery<Maybe<Dto>>
    {
        public Guid CurveRecipeId { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}
