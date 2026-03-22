namespace Magnise.Api.DTO.Response;

public class AssetPriceResponse
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }
}