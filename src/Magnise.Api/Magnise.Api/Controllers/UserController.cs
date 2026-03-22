using Magnise.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Magnise.Api.Controllers;

[ApiController]
[Route("api/assets")]
public class UserController(IAssetService assetService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAssets()
    {
        var assets = await assetService.GetAllAssetsAsync();
        return Ok(assets);
    }

    [HttpGet("price")]
    public async Task<IActionResult> GetAssetPrice([FromQuery] string symbol)
    {
        var result = await assetService.GetAssetPriceAsync(symbol);

        if (result is null)
            return NotFound($"Asset '{symbol}' not found");

        return Ok(result);
    }
}