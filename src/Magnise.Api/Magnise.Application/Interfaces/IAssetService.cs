using Magnise.Application;

namespace Magnise.Domain.Interfaces.Services;

public interface IAssetService
{
    public Task<List<string>> GetAllAssetsAsync();
    public Task<AssetPriceResponse?> GetAssetPriceAsync(string symbol);
}