using System;
using Common.Core;
using Newtonsoft.Json.Linq;

namespace CurveRecipes.Service.Features.AddTransformation
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
        public string? TransformationName { get; set; }
        public JObject? Transformation { get; set; }
    }
}
