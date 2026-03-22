using Microsoft.Extensions.Hosting;

namespace Magnise.Infrastructure.Fintacharts;

public class WsBackgroundService(FintachartsWsClient wsClient) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
        => wsClient.StartAsync(ct);
}