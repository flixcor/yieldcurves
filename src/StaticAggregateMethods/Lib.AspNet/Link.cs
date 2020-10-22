using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lib.AspNet
{
    public record Link(string Href, string Method)
    {
        public HydraClass? Expects { get; init; }
        public string? Schema { get; init; }
    }

    public class HydraClass
    {
        [JsonPropertyName("@id")]
        public string? @Id { get; init; }
        [JsonPropertyName("@type")]
        public string @Type { get; } = "hydra:Class";
        public IReadOnlyCollection<SupportedProperty> SupportedProperty { get; init; } = Array.Empty<SupportedProperty>();
        public string? Title { get; init; }
    }

    public class SupportedProperty
    {
        [JsonPropertyName("@type")]
        public string @Type { get; } = "SupportedProperty";
        public bool Required { get; init; } = true;
        public string? Title { get; init; }
        public dynamic? Default { get; init; }
    }
}
