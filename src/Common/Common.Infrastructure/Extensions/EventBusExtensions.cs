using Common.Core;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Extensions
{
    public static class EventBusExtensions
    {
        public static IEventBus WithLogging(this IEventBus bus)
        {
            return new EventBusLogger(bus);
        }
    }
}
