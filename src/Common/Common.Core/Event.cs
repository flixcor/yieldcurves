using System;
using LanguageExt;

namespace Common.Core
{
    public interface IEvent : IMessage
    {
        Guid Id { get; }
        int Version { get; }
    }

    public class Event : Record<Event>, IEvent
    {
        protected Event(Guid id)
        {
            Id = id;
        }

        public Guid Id { get;}
        public int Version { get; internal protected set; }
    }
}
