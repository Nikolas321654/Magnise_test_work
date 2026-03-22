using Magnise.Domain.Interfaces.Repositories;
using Magnise.Domain.Interfaces.Services;

namespace Magnise.Application.Services;

public class AssetService(IAssetRepository repository) : IAssetService
{
    public async Task<List<string>> GetAllAssetsAsync()
    {
        return await repository.GetAllAssert();
    }

    public async Task<AssetPriceResponse?> GetAssetPriceAsync(string symbol)
    {
        var asset = await repository.GetAssetPriceBySymbol(symbol);

        if (asset is null)
            return null;

        return new AssetPriceResponse
        {
            Symbol = asset.Symbol,
            Price = asset.AssetPrice.Price,
            UpdatedAt = asset.AssetPrice.Date
        };
    }
}