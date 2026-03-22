using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class WsSubscribeMessage
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

    [JsonPropertyName("instrumentId")] public string InstrumentId { get; set; } = string.Empty;

    [JsonPropertyName("provider")] public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("subscribe")] public bool Subscribe { get; set; }

    [JsonPropertyName("kinds")] public List<string> Kinds { get; set; } = [];
}