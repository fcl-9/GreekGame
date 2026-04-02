namespace GreekGame.API.Application;

public interface ITradeService
{
    Task<bool> BuyFood(Guid cityId, int amount);
    Task<bool> SellFood(Guid cityId, int amount);
}
