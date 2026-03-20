using Magnise.Domain.Entities;

namespace Magnise.Domain.Interfaces.Repositories;

public interface IAssetPriceRepository
{
    public Task<IEnumerable<AssetPriceEntity>> GetAllAssetPrices();
    public Task<IEnumerable<AssetPriceEntity>> GetAssetPricesByDate(DateTime date);
    public Task<AssetPriceEntity> CreateAssetPrice(AssetPriceEntity assetPrice);
    public Task<AssetPriceEntity> GetAssetPriceByAssetId(Guid assetId);
    public Task<AssetPriceEntity> UpdateAssetPrice(Guid assetId);
}