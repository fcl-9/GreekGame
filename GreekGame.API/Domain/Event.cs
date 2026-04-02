namespace GreekGame.API.Domain;

public class Event
{
    public Guid Id { get; set; }
    public EventType Type { get; set; }
    public Guid CityId { get; set; }
    public int DurationTicks { get; set; }
    public int RemainingTicks { get; set; }
}
