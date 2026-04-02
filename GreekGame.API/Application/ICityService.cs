using GreekGame.API.Domain;

namespace GreekGame.API.Application;

public interface ICityService
{
    Task<City> CreateCityAsync();
    Task<bool> BuildBuildingAsync(Guid cityId, BuildingType buildingType);
    Task<bool> UpgradeBuildingAsync(Guid buildingId);
    Task<City?> GetCityAsync(Guid id);
}
