using Magnise.Domain.Entities;
using Magnise.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Magnise.Infrastructure;

public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options)
{
    public DbSet<AssetEntity> Assets { get; set; }
    public DbSet<AssetPriceEntity> AssetPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetConfiguration());
        modelBuilder.ApplyConfiguration(new AssetPriceConfiguration());
    }
}