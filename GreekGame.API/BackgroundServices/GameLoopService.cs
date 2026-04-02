using GreekGame.API.Domain;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GreekGame.API.BackgroundServices;

public class GameLoopService : BackgroundService
{
    private readonly IServiceProvider _services;

    public GameLoopService(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();

            var cities = await db.Cities.ToListAsync(stoppingToken);

            foreach (var city in cities)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - city.LastUpdated).TotalSeconds;

                // 🧠 Basic simulation
                var ticks = (int)(seconds / 5); // 1 tick per 5 seconds
                if (ticks <= 0) continue;

                for (var i = 0; i < ticks; i++) UpdateCity(city);

                city.LastUpdated = now;
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(5000, stoppingToken);
        }
    }

    private void UpdateCity(City city)
    {
        var farms = city.Buildings.Where(b => b.Type == BuildingType.Farm);
        // 🌾 Food production & consumption
        city.Food += farms.Sum(f => 5 * f.Level);
        city.Food -= city.Population;

        var houses = city.Buildings.Where(b => b.Type == BuildingType.House);
        var maxPopulation = houses.Sum(h => 5 * h.Level);
        // 👥 Population change
        if (city.Food >= city.Population && city.Population < maxPopulation)
            city.Population += 1;
        else
            city.Population = Math.Max(0, city.Population - 1);

        // 💰 Economy
        city.Money += city.Population * 0.5m;
    }

    private void TriggerRandomEvent(City city)
    {
        var rand = new Random();
        if (rand.NextDouble() < 0.1)
        {
            var parsingResult = Enum.TryParse<EventType>(rand.Next(0, 3).ToString(), out var eventType);
            if (!parsingResult) throw new InvalidEventTypeException("Failed to parse random event type.");
            city.ActiveEvents.Add(new Event
            {
                Id = Guid.NewGuid(),
                CityId = city.Id,
                Type = eventType,
                DurationTicks = 3,
                RemainingTicks = 3
            });
        }
    }

    private void UpdateMarket(Market market)
    {
        if (market.TotalSupply == 0) market.TotalSupply = 1;
        if (market.TotalDemand == 0) market.TotalDemand = 1;

        var ratio = (decimal)market.TotalDemand / market.TotalSupply;

        market.FoodPrice *= ratio;

        // Clamp price (avoid insane values)
        market.FoodPrice = Math.Clamp(market.FoodPrice, 1, 100);

        // Reset for next tick
        market.TotalSupply = 0;
        market.TotalDemand = 0;
    }
}
