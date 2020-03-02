namespace Common.Core
{
    public class EventPosition : ReadObject
    {
        private EventPosition() { }

        public EventPosition(long commitPosition, long preparePosition, string applicationName)
        {
            ApplicationName = applicationName;
            CommitPosition = commitPosition;
            PreparePosition = preparePosition;
        }

        public string? ApplicationName { get; private set; }
        public long CommitPosition { get; private set; }
        public long PreparePosition { get; private set; }
    }
}
