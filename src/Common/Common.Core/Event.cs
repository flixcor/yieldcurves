using System;

namespace Common.Core
{
    public abstract class Event : IEvent
    {
        protected Event(Guid id, int version = 0)
        {
            Id = id;
            Version = version;
        }

        public Guid Id { get; }
        public int Version { get; private set; }

        public virtual Event WithVersion(int version)
        {
            var clone = (Event)MemberwiseClone();
            clone.Version = version;
            return clone;
        }
    }
}
