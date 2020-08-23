namespace ExampleService.Domain
{
    public static class Commands
    {
        public record NameAndAddInstrument
        {
            public string? Name { get; init; }
            public string? Instrument { get; init; }
        }
    }
}
