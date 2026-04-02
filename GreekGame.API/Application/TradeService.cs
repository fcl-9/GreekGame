using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GreekGame.API.Application;

public class TradeService : ITradeService
{
    private readonly GameDbContext _db;

    public TradeService(GameDbContext db) => _db = db;

    public async Task<bool> BuyFood(Guid cityId, int amount)
    {
        var city = await _db.Cities.FindAsync(cityId);
        var market = await _db.Markets.FirstAsync();

        if (city == null)
            return false;

        var cost = market.FoodPrice * amount;

        if (city.Money < cost)
            return false;

        city.Money -= cost;
        city.Food += amount;

        market.TotalDemand += amount;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SellFood(Guid cityId, int amount)
    {
        var city = await _db.Cities.FindAsync(cityId);
        var market = await _db.Markets.FirstAsync();

        if (city == null)
            return false;

        if (city.Food < amount)
            return false;

        var revenue = market.FoodPrice * amount;

        city.Food -= amount;
        city.Money += revenue;

        market.TotalSupply += amount;

        await _db.SaveChangesAsync();
        return true;
    }
}
