using Common.Core;
using EventStore.Client;

namespace Common.EventStore.Lib.GES
{
    public static class EventPositionExtensions
    {
        public static Position ToEventStorePosition(this EventPosition eventPosition)
        {
            return eventPosition == null
                ? Position.Start
                : new Position((ulong)eventPosition.CommitPosition, (ulong)eventPosition.PreparePosition);
        }

        public static EventPosition ToEventPosition(this Position position, string applicationName)
        {
            return new EventPosition((long)position.CommitPosition, (long)position.PreparePosition, applicationName);
        }
    }
}
