using System.Runtime.InteropServices;
using Magnise.Domain.Entities;
using Magnise.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Magnise.Infrastructure.Repositories;

public class AssetRepository(AssetDbContext context) : IAssetRepository
{
    public async Task<List<string>> GetAllAssert()
    {
        return await context.Assets.Select(x => x.Symbol).ToListAsync();
    }

    public async Task<AssetEntity?>
        GetAssetPriceBySymbol(string symbol)
    {
        return await context.Assets
            .Include(x => x.AssetPrice)
            .FirstOrDefaultAsync(x => x.Symbol == symbol);
    }

    public async Task CreateAsset(AssetEntity asset)
    {
        await context.AddAsync(asset);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePrice(string symbol, decimal price)
    {
        var asset = await context.Assets
            .Include(x => x.AssetPrice)
            .FirstOrDefaultAsync(x => x.Symbol == symbol);

        if (asset is null) return;

        if (asset.AssetPrice is null)
        {
            asset.AssetPrice = new AssetPriceEntity
            {
                AssetId = asset.Id,
                Price = price,
                Date = DateTime.UtcNow
            };
            context.Add(asset.AssetPrice);
        }
        else
        {
            asset.AssetPrice.Price = price;
            asset.AssetPrice.Date = DateTime.UtcNow;
            context.Update(asset.AssetPrice);
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<AssetEntity>> GetAllAssetsWithId()
    {
        return await context.Assets.ToListAsync();
    }

    public async Task<AssetEntity?> GetAssetById(string id)
    {
        return await context.Assets.FirstOrDefaultAsync(x => x.Id == id);
    }
}