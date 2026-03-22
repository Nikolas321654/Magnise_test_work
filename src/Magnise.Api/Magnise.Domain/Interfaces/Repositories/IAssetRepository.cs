using Magnise.Domain.Entities;

namespace Magnise.Domain.Interfaces.Repositories;

public interface IAssetRepository
{
    public Task UpdatePrice(string symbol, decimal price);
    public Task CreateAsset(AssetEntity asset);
    public Task<AssetEntity?> GetAssetPriceBySymbol(string symbol);
    public Task<List<string>> GetAllAssert();
    public Task<List<AssetEntity>> GetAllAssetsWithId();
    public Task<AssetEntity?> GetAssetById(string id);
}