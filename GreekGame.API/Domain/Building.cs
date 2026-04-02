namespace GreekGame.API.Domain;

public class Building
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public BuildingType Type { get; set; }
    public byte Level { get; set; }
    public DateTime LastUpdated { get; set; } 
}