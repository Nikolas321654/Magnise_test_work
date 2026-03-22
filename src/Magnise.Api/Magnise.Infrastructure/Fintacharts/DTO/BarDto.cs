using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class BarDto
{
    [JsonPropertyName("t")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("c")] 
    public decimal Close { get; set; }

    [JsonPropertyName("o")] 
    public decimal Open { get; set; }

    [JsonPropertyName("h")] 
    public decimal High { get; set; }

    [JsonPropertyName("l")] 
    public decimal Low { get; set; }
}