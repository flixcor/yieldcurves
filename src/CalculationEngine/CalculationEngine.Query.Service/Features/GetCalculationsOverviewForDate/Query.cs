using System.Collections.Generic;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculationsOverviewForDate
{
    public class Query : IQuery<Maybe<Dto>>
    {
        public string AsOfDate { get; set; }
    }
}
