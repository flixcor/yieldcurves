using System;

namespace Common.Core
{
    public abstract class ReadObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
