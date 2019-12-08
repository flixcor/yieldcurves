using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Core
{
    public abstract class ReadObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
