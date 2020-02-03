using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Events
{
    internal partial class Uuid
    {
        public Uuid(Guid guid) => Value = guid.ToString();

        public static implicit operator Guid(Uuid uuid) => Guid.Parse(uuid.Value);
        public static implicit operator Uuid(Guid guid) => new Uuid(guid);
    }
}
