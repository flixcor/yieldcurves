

namespace Common.EventStore
{
    public class EventReply
    {
        public ulong Position { get; set; }
        public string Type { get; set; }
        public dynamic Payload { get; set; }
    }
}
