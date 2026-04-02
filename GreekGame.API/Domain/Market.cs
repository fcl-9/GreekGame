namespace GreekGame.API.Domain;

public class Market
{
    public Guid Id { get; set; }

    public decimal FoodPrice { get; set; } = 10;

    public int TotalSupply { get; set; }
    public int TotalDemand { get; set; }
}
