using System.Collections.Generic;

namespace ExampleService.Shared
{
    public static class Handlers
    {
        public delegate IEnumerable<object> CommandHandler<CommandType, StateType>(CommandType command, StateType state);
        public delegate StateType EventApplier<EventType, StateType>(EventType @event, StateType state);
    }
}
