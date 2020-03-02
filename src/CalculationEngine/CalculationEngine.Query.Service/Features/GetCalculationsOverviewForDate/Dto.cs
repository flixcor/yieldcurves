using System;
using System.Collections.Generic;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculationsOverviewForDate
{
    public class Dto : ReadObject
    {
        public string? AsOfDate { get; set; }
        public ICollection<RecipeDto> Recipes { get; set; } = Array.Empty<RecipeDto>();
    }

    public class RecipeDto : ReadObject
    {
        public string? Name { get; set; }
    }
}
