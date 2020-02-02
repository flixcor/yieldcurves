﻿using System;
using Common.Core;

namespace CalculationEngine.Query.Service.Features.GetCalculatedCurveDetail
{
    public class Query : IQuery<Dto?>
    {
        public Guid CurveRecipeId { get; set; }
        public string AsOfDate { get; set; }
    }
}
