namespace Common.EventStore
{
    public class EventRequest
    {
        public string[] EventTypes { get; set; }
        public string StreamName { get; set; }
        public long PreparePosition { get; set; }
        public long CommitPosition { get; set; }
    }
}
