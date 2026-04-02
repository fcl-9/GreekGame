namespace GreekGame.API.Application;

using GreekGame.API.Domain;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class CityService : ICityService
{
    private readonly GameDbContext _db;

    public CityService(GameDbContext db)
    {
        this._db = db;
    }

    public async Task<City> CreateCityAsync()
    {
        var city = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 200,
            LastUpdated = DateTime.UtcNow
        };

        _db.Cities.Add(city);
        await _db.SaveChangesAsync();

        return city;
    }

    public async Task<bool> BuildBuildingAsync(Guid cityId, BuildingType buildingType)
    {
        var city = await _db.Cities
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == cityId);

        if (city is null)
        {
            return false;
        }

        var cost = BuildingUpgrades.GetBuildingCost(buildingType);

        if (city.Money <= cost)
        {
            return false;
        }

        city.Money -= cost;

        city.Buildings.Add(new Building()
        {
            Id = Guid.NewGuid(),
            CityId = cityId,
            Type = buildingType,
            Level = 1
        });
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpgradeBuildingAsync(Guid buildingId)
    {
        var building = await _db.Buildings.FindAsync(buildingId);

        if (building is null)
        {
            return false;
        }

        var cost = BuildingUpgrades.GetUpgradeCost(building.Type, building.Level);

        var city = await _db.Cities.FindAsync(building.CityId);

        if (city is null || city.Money < cost)
        {
            return false;
        }

        city.Money -= cost;
        building.Level += 1;

        await _db.SaveChangesAsync();

        return true;
    }

    public Task<City?> GetCityAsync(Guid id)
    {
        return _db.Cities.FindAsync(id).AsTask();
    }
}
