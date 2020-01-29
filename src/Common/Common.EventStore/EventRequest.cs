namespace Common.EventStore
{
    public class EventRequest
    {
        public string[] EventTypes { get; set; }
        public string StreamName { get; set; }
        public ulong Position { get; set; }
    }
}
