using Magnise.Domain.Entities;
using Magnise.Domain.Interfaces.Repositories;

namespace Magnise.Infrastructure.Repositories;

public class AssetRepository(AssetDbContext context) : IAssetRepository
{
    public async Task<IEnumerable<AssetEntity>> GetAllAssert()
    {
        throw new NotImplementedException();
    }

    public async Task<AssetEntity> GetAssetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetEntity> GetAssetBySymbol(string symbol)
    {
        throw new NotImplementedException();
    }

    public async Task<AssetEntity> CreateAsset(AssetEntity asset)
    {
        throw new NotImplementedException();
    }
}