using Common.Core;

namespace Common.EventStore
{
    public class EventReply
    {
        public long PreparePosition { get; set; }
        public long CommitPosition { get; set; }
        public string EventType { get; set; }
        public byte[] Payload { get; set; }
    }
}
