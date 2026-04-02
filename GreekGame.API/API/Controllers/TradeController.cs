using GreekGame.API.Application;
using Microsoft.AspNetCore.Mvc;

namespace GreekGame.API.API.Controllers;

[ApiController]
[Route("api/trade")]
public class TradeController : ControllerBase
{
    private readonly ITradeService _tradeService;

    public TradeController(ITradeService tradeService) => _tradeService = tradeService;

    [HttpPost("{cityId}/buy")]
    public async Task<IActionResult> Buy(Guid cityId, int amount)
    {
        var success = await _tradeService.BuyFood(cityId, amount);
        return success ? Ok() : BadRequest();
    }

    [HttpPost("{cityId}/sell")]
    public async Task<IActionResult> Sell(Guid cityId, int amount)
    {
        var success = await _tradeService.SellFood(cityId, amount);
        return success ? Ok() : BadRequest();
    }
}
