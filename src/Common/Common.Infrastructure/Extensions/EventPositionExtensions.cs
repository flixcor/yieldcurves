using Common.Core;
using EventStore.Client;

namespace Common.Infrastructure.Extensions
{
    public static class EventPositionExtensions
    {
        public static Position ToEventStorePosition(this EventPosition eventPosition)
        {
            return eventPosition == null 
                ? Position.Start
                : new Position(eventPosition.CommitPosition, eventPosition.PreparePosition);
        }

        public static EventPosition ToEventPosition(this Position? position, string applicationName)
        {
            return position == null 
                ? null : 
                new EventPosition(position.Value.CommitPosition, position.Value.PreparePosition, applicationName);
        }
    }
}
