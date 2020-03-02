using System.Collections.Generic;
using Common.Core;

namespace CurveRecipes.Service.Features.CreateCurveRecipe
{
    public class MarketCurveDto : ReadObject
    {
        public string? Name { get; set; }
        public IList<string> Tenors { get; set; } = new List<string>();
    }
}
