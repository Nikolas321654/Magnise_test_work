using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class InstrumentsResponse
{
    [JsonPropertyName("data")]
    public List<InstrumentDto> Data { get; set; } = [];
}