using System;

namespace Common.Core
{
    public abstract class ReadObject
    {
        public NonEmptyGuid Id { get; set; } = Guid.NewGuid().NonEmpty();
    }
}
