using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Magnise.Infrastructure;

public class AssetDbContextFactory : IDesignTimeDbContextFactory<AssetDbContext>
{
    public AssetDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AssetDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=AssetDb;Username=postgres;Password=magnise");

        return new AssetDbContext(optionsBuilder.Options);
    }
}