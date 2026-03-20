using Magnise.Domain.Entities;

namespace Magnise.Domain.Interfaces.Repositories;

public interface IAssetRepository
{
    public Task<IEnumerable<AssetEntity>> GetAllAssert();
    public Task<AssetEntity> GetAssetById(Guid id);
    public Task<AssetEntity> GetAssetBySymbol(string symbol);
    public Task<AssetEntity> CreateAsset(AssetEntity asset);
}