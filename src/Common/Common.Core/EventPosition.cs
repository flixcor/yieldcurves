namespace Common.Core
{
    public class EventPosition : ReadObject
    {
        public EventPosition(long commitPosition, long preparePosition, string applicationName)
        {
            ApplicationName = applicationName;
            CommitPosition = commitPosition;
            PreparePosition = preparePosition;
        }

        public string ApplicationName { get; }
        public long CommitPosition { get; }
        public long PreparePosition { get; }
    }
}