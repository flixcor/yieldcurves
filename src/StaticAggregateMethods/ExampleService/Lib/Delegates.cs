﻿using System.Collections.Generic;

namespace ExampleService.Shared
{
    public static class Delegates
    {
        public delegate IEnumerable<object> Handle<State, Command>(State state, Command command);
        public delegate State When<State, Event>(State state, Event @event);
        public delegate string GetStreamName(string id);
    }
}
