namespace Magnise.Domain.Entities;

public class AssetPriceEntity
{
    public Guid AssetId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    
    public AssetEntity Asset { get; set; } = null!;
}