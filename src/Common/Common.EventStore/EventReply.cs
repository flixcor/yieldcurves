

namespace Common.EventStore
{
    public class EventReply
    {
        public long Position { get; set; }
        public string Type { get; set; }
        public dynamic Payload { get; set; }
    }
}
