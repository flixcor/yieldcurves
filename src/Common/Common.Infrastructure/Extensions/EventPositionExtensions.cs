using Common.Core;
using EventStore.ClientAPI;

namespace Common.Infrastructure.Extensions
{
    public static class EventPositionExtensions
    {
        public static Position? ToEventStorePosition(this EventPosition eventPosition)
        {
            return eventPosition == null 
                ? AllCheckpoint.AllStart 
                : (Position?)new Position(eventPosition.CommitPosition, eventPosition.PreparePosition);
        }

        public static EventPosition ToEventPosition(this Position? position, string applicationName)
        {
            return position == null 
                ? null : 
                new EventPosition(position.Value.CommitPosition, position.Value.PreparePosition, applicationName);
        }
    }
}
