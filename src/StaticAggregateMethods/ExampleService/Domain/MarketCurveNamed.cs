﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Domain
{
    public record MarketCurveNamed
    {
        public string Name { get; init; }
    }
}