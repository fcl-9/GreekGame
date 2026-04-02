namespace GreekGame.API.Application;

using GreekGame.API.Domain;

public interface ICityService
{
    Task<City> CreateCityAsync();
    Task<bool> BuildBuildingAsync(Guid cityId, BuildingType buildingType);
    public Task<bool> UpgradeBuildingAsync(Guid buildingId);
    Task<City?> GetCityAsync(Guid id);
}
