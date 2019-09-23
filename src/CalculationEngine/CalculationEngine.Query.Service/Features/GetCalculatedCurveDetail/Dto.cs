using System;
using System.Collections.Generic;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculatedCurveDetail
{
    public class Dto : ReadObject
    {
        public Guid CurveRecipeId { get; set; }
        public string CurveRecipeName { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime AsAtDate { get; set; }
        public ICollection<Point> Points { get; set; }
    }
}
