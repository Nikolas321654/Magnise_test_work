using Magnise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magnise.Infrastructure.Configurations;

public class AssetPriceConfiguration : IEntityTypeConfiguration<AssetPriceEntity>
{
    public void Configure(EntityTypeBuilder<AssetPriceEntity> builder)
    {
        builder.ToTable("AssetPrices");
        builder.HasKey(x => x.AssetId);
        builder.Property(x => x.AssetId).ValueGeneratedNever();

        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Price).IsRequired();
        builder.HasIndex(x => x.AssetId);
    }
}