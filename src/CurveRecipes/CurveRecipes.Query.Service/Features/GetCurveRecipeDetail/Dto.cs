using System.Collections.Generic;
using Common.Core;

namespace CurveRecipes.Query.Service.Features.GetCurveRecipeDetail
{
    public class Dto : ReadObject
    {
        public string Name { get; set; }
        public IList<TransformationDto> Transformations { get; set; } = new List<TransformationDto>();
    }

    public class TransformationDto
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public IList<ParameterDto> Parameters { get; set; } = new List<ParameterDto>();
    }

    public class ParameterDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class MarketCurveNamePartDto : ReadObject
    {
        public string Value { get; set; }
    }
}
