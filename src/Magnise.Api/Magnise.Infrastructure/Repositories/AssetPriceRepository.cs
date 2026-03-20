using Magnise.Domain.Entities;
using Magnise.Domain.Interfaces.Repositories;

namespace Magnise.Infrastructure.Repositories;

public class AssetPriceRepository(AssetDbContext context) : IAssetPriceRepository
{
    public async Task<IEnumerable<AssetPriceEntity>> GetAllAssetPrices()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<AssetPriceEntity>> GetAssetPricesByDate(DateTime date)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetPriceEntity> CreateAssetPrice(AssetPriceEntity assetPrice)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetPriceEntity> GetAssetPriceByAssetId(Guid assetId)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetPriceEntity> UpdateAssetPrice(Guid assetId)
    {
        throw new NotImplementedException();
    }
}