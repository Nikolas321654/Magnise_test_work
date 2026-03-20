namespace Magnise.Domain.Entities;

public class AssetEntity
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }

    public AssetPriceEntity AssetPrice { get; set; } = null!;
}