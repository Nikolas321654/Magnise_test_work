using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class InstrumentDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;
}