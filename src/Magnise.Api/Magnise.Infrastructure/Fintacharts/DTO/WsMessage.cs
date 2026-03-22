using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts;

public class WsMessage
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("instrumentId")] public string InstrumentId { get; set; } = string.Empty;

    [JsonPropertyName("last")] public WsLastPrice? Last { get; set; }
}