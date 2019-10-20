using System.Collections.Generic;
using System.Linq;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculationsOverviewForDate
{
    public class Dto : ReadObject
    {
        public string AsOfDate { get; set; }
        public IEnumerable<RecipeDto> Recipes { get; set; } = Enumerable.Empty<RecipeDto>();
    }

    public class RecipeDto : ReadObject
    {
        public string Name { get; set; }
    }
}
