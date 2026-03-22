using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Magnise.Domain.Interfaces.Repositories;
using Magnise.Infrastructure.Fintacharts.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magnise.Infrastructure.Fintacharts;

public class FintachartsWsClient(
    FintachartsAuthService authService,
    IServiceScopeFactory scopeFactory,
    IConfiguration config,
    ILogger<FintachartsWsClient> logger)
{
    private ClientWebSocket _ws = new();

    public async Task StartAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await ConnectAsync(ct);
                await SubscribeToAllAsync(ct);
                await ReceiveLoopAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "WS disconnected. Reconnecting in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }

    private async Task ConnectAsync(CancellationToken ct)
    {
        _ws.Dispose();
        _ws = new ClientWebSocket();

        var token = await authService.GetAccessTokenAsync(ct);
        var wssUrl = config["Fintacharts:WssUrl"]!;
        var uri = new Uri($"{wssUrl}/api/streaming/ws/v1/realtime?token={token}");

        await _ws.ConnectAsync(uri, ct);
        logger.LogInformation("Connected to Fintacharts WebSocket");
    }

    private async Task SubscribeToAllAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var assets = await repo.GetAllAssetsWithId();

        foreach (var asset in assets)
        {
            var message = new WsSubscribeMessage
            {
                Type = "l1-subscription",
                Id = Guid.NewGuid().ToString(),
                InstrumentId = asset.Id,
                Provider = "oanda",
                Subscribe = true,
                Kinds = ["last"]
            };

            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
            logger.LogInformation("Subscribed to {Symbol}", asset.Symbol);
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        var buffer = new byte[4096];

        while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var result = await _ws.ReceiveAsync(buffer, ct);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, ct);
                break;
            }

            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await HandleMessageAsync(json);
        }
    }

    private async Task HandleMessageAsync(string json)
    {
        try
        {
            var message = JsonSerializer.Deserialize<WsMessage>(json);

            if (message?.Type != "l1-update" || message.Last is null)
                return;

            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAssetRepository>();

            var asset = await repo.GetAssetById(message.InstrumentId);
            if (asset is null)
            {
                logger.LogWarning("Unknown instrumentId: {Id}", message.InstrumentId);
                return;
            }

            await repo.UpdatePrice(asset.Symbol, message.Last.Price);
            logger.LogDebug("Updated {Symbol} → {Price}", asset.Symbol, message.Last.Price);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle WS message: {Json}", json);
        }
    }
}