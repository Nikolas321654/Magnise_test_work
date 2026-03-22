using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts;

public class WsLastPrice
{
    [JsonPropertyName("price")] public decimal Price { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}