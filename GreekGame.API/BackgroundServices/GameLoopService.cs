namespace GreekGame.API.BackgroundServices;

using GreekGame.API.Domain;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class GameLoopService : BackgroundService
{
    private readonly IServiceProvider _services;

    public GameLoopService(IServiceProvider services)
    {
        this._services = services;
    }

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
                int ticks = (int)(seconds / 5); // 1 tick per 5 seconds
                if (ticks <= 0) continue;

                for (int i = 0; i < ticks; i++)
                {
                    UpdateCity(city);
                }

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
}
