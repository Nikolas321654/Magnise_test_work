using Magnise.Application.Services;
using Magnise.Domain.Interfaces.Repositories;
using Magnise.Domain.Interfaces.Services;
using Magnise.Infrastructure;
using Magnise.Infrastructure.Fintacharts;
using Magnise.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AssetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(AssetDbContext))));

builder.Services.AddScoped<IAssetRepository, AssetRepository>();

var fintachartsBase = builder.Configuration["Fintacharts:BaseUrl"]!;

builder.Services.AddHttpClient("FintachartsAuth", client =>
    client.BaseAddress = new Uri(fintachartsBase));

builder.Services.AddSingleton<FintachartsAuthService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("FintachartsAuth");
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<FintachartsAuthService>>();
    return new FintachartsAuthService(httpClient, config, logger);
});

builder.Services.AddHttpClient("FintachartsRest", client =>
    client.BaseAddress = new Uri(fintachartsBase));

builder.Services.AddScoped<FintachartsRestClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("FintachartsRest");
    var auth = sp.GetRequiredService<FintachartsAuthService>();
    return new FintachartsRestClient(httpClient, auth);
});

builder.Services.AddSingleton<FintachartsWsClient>();
builder.Services.AddHostedService<WsBackgroundService>();

builder.Services.AddScoped<IAssetService, AssetService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning("DB not ready. Retries left: {Retries}. Error: {Error}", retries, ex.Message);
            await Task.Delay(3000);
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var restClient = scope.ServiceProvider.GetRequiredService<FintachartsRestClient>();
    var repository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var instruments = await restClient.GetInstrumentsAsync();

        foreach (var instrument in instruments)
        {
            var exists = await repository.GetAssetById(instrument.Id);
            if (exists is not null) continue;

            await repository.CreateAsset(new Magnise.Domain.Entities.AssetEntity
            {
                Id = instrument.Id,
                Symbol = instrument.Symbol
            });
        }

        logger.LogInformation("Seeded {Count} instruments", instruments.Count);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to seed instruments");
    }
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapControllers();
app.Run();