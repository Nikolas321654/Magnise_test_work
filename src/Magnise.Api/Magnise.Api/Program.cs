using Magnise.Domain.Interfaces.Repositories;
using Magnise.Infrastructure;
using Magnise.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IAssetPriceRepository, AssetPriceRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

builder.Services.AddDbContext<AssetDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(AssetDbContext)));
});


var app = builder.Build();
app.MapControllers();

// app.UseHttpsRedirection();

app.Run();