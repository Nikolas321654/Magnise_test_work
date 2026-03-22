using System.Net.Http.Headers;
using System.Text.Json;
using Magnise.Infrastructure.Fintacharts.DTO;

namespace Magnise.Infrastructure.Fintacharts;

public class FintachartsRestClient(
    HttpClient httpClient,
    FintachartsAuthService authService)
{
    public async Task<List<InstrumentDto>> GetInstrumentsAsync(
        string provider = "oanda",
        string kind = "forex",
        CancellationToken ct = default)
    {
        await SetAuthHeaderAsync(ct);

        var url = $"/api/instruments/v1/instruments?provider={provider}&kind={kind}";
        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<InstrumentsResponse>(json);

        return result?.Data ?? [];
    }

    public async Task<BarDto?> GetLatestPriceAsync(
        string instrumentId,
        string provider = "oanda",
        CancellationToken ct = default)
    {
        await SetAuthHeaderAsync(ct);

        var url = $"/api/bars/v1/bars/count-back" +
                  $"?instrumentId={instrumentId}" +
                  $"&provider={provider}" +
                  $"&interval=1&periodicity=minute&barsCount=1";

        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<BarsResponse>(json);

        return result?.Data?.FirstOrDefault();
    }

    private async Task SetAuthHeaderAsync(CancellationToken ct)
    {
        var token = await authService.GetAccessTokenAsync(ct);
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}