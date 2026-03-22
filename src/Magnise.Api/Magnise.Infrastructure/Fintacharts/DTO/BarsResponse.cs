using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class BarsResponse
{
    [JsonPropertyName("data")]
    public List<BarDto> Data { get; set; } = [];
}