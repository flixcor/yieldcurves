namespace Common.Core
{
    public class EventPosition : ReadObject
    {
        private EventPosition() { }

        public EventPosition(ulong commitPosition, ulong preparePosition, string applicationName)
        {
            ApplicationName = applicationName;
            CommitPosition = commitPosition;
            PreparePosition = preparePosition;
        }

        public string ApplicationName { get; private set; }
        public ulong CommitPosition { get; private set; }
        public ulong PreparePosition { get; private set; }
    }
}
