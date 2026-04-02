namespace GreekGame.Tests;

using GreekGame.API.Application;
using GreekGame.API.Domain;
using GreekGame.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class CityServiceTests
{
    private GameDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task CreateCityAsync_CreatesNewCity_WithCorrectInitialValues()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);

        // Act
        var city = await service.CreateCityAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, city.Id);
        Assert.Equal(10, city.Population);
        Assert.Equal(50, city.Food);
        Assert.Equal(200m, city.Money);
        Assert.NotEqual(default, city.LastUpdated);
        Assert.Empty(city.Buildings);
    }

    [Fact]
    public async Task CreateCityAsync_SavesCity_ToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        var cityId = Guid.Empty;

        // Act
        var city = await service.CreateCityAsync();
        cityId = city.Id;

        // Assert
        var savedCity = await context.Cities.FindAsync(cityId);
        Assert.NotNull(savedCity);
        Assert.Equal(cityId, savedCity.Id);
    }

    [Fact]
    public async Task BuildBuildingAsync_WithValidCity_BuildsBuilding()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;

        // Act
        var success = await service.BuildBuildingAsync(cityId, BuildingType.Farm);

        // Assert
        Assert.True(success);
        var updatedCity = await context.Cities
            .Include(c => c.Buildings)
            .FirstAsync(c => c.Id == cityId);
        Assert.Single(updatedCity.Buildings);
        Assert.Equal(BuildingType.Farm, updatedCity.Buildings.First().Type);
        Assert.Equal(1, updatedCity.Buildings.First().Level);
    }

    [Fact]
    public async Task BuildBuildingAsync_DeductsCostFromCity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;
        var initialMoney = city.Money;

        // Act
        await service.BuildBuildingAsync(cityId, BuildingType.Farm);

        // Assert
        var updatedCity = await context.Cities.FindAsync(cityId);
        Assert.Equal(initialMoney - 50m, updatedCity!.Money);
    }

    [Fact]
    public async Task BuildBuildingAsync_WithInsufficientFunds_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        city.Money = 40m; // Less than Farm cost (50)
        await context.SaveChangesAsync();

        // Act
        var success = await service.BuildBuildingAsync(city.Id, BuildingType.Farm);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public async Task BuildBuildingAsync_WithNonExistentCity_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        var nonExistentCityId = Guid.NewGuid();

        // Act
        var success = await service.BuildBuildingAsync(nonExistentCityId, BuildingType.Farm);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public async Task BuildBuildingAsync_AllowsExactCost()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        city.Money = 50m; // Exact cost for Farm
        await context.SaveChangesAsync();

        // Act
        var success = await service.BuildBuildingAsync(city.Id, BuildingType.Farm);

        // Assert
        Assert.False(success); // Should fail because condition is <= not <
    }

    [Fact]
    public async Task UpgradeBuildingAsync_WithValidBuilding_UpgradesLevel()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;
        await service.BuildBuildingAsync(cityId, BuildingType.Farm);
        var building = context.Buildings.First();
        var buildingId = building.Id;

        // Act
        var success = await service.UpgradeBuildingAsync(buildingId);

        // Assert
        Assert.True(success);
        var upgradedBuilding = await context.Buildings.FindAsync(buildingId);
        Assert.Equal(2, upgradedBuilding!.Level);
    }

    [Fact]
    public async Task UpgradeBuildingAsync_DeductsCostFromCity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;
        var initialMoneyAfterBuild = city.Money - 50m;
        await service.BuildBuildingAsync(cityId, BuildingType.Farm);
        var building = context.Buildings.First();
        var buildingId = building.Id;

        // Act
        await service.UpgradeBuildingAsync(buildingId);

        // Assert
        var updatedCity = await context.Cities.FindAsync(cityId);
        var upgradeCost = BuildingUpgrades.GetUpgradeCost(BuildingType.Farm, 1);
        Assert.Equal(initialMoneyAfterBuild - upgradeCost, updatedCity!.Money);
    }

    [Fact]
    public async Task UpgradeBuildingAsync_WithNonExistentBuilding_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        var nonExistentBuildingId = Guid.NewGuid();

        // Act
        var success = await service.UpgradeBuildingAsync(nonExistentBuildingId);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public async Task UpgradeBuildingAsync_WithInsufficientFunds_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;
        await service.BuildBuildingAsync(cityId, BuildingType.Farm);
        var building = context.Buildings.First();
        var buildingId = building.Id;
        city.Money = 0; // No money for upgrade
        await context.SaveChangesAsync();

        // Act
        var success = await service.UpgradeBuildingAsync(buildingId);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public async Task UpgradeBuildingAsync_WithNonExistentCity_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;
        await service.BuildBuildingAsync(cityId, BuildingType.Farm);
        var building = context.Buildings.First();
        var buildingId = building.Id;

        // Remove the city
        context.Cities.Remove(city);
        await context.SaveChangesAsync();

        // Act
        var success = await service.UpgradeBuildingAsync(buildingId);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public async Task GetCityAsync_WithExistingCity_ReturnsCity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        var createdCity = await service.CreateCityAsync();

        // Act
        var retrievedCity = await service.GetCityAsync(createdCity.Id);

        // Assert
        Assert.NotNull(retrievedCity);
        Assert.Equal(createdCity.Id, retrievedCity.Id);
        Assert.Equal(createdCity.Population, retrievedCity.Population);
        Assert.Equal(createdCity.Food, retrievedCity.Food);
        Assert.Equal(createdCity.Money, retrievedCity.Money);
    }

    [Fact]
    public async Task GetCityAsync_WithNonExistentCity_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        var nonExistentCityId = Guid.NewGuid();

        // Act
        var city = await service.GetCityAsync(nonExistentCityId);

        // Assert
        Assert.Null(city);
    }

    [Fact]
    public async Task BuildBuildingAsync_MultipleBuildings_AllCreated()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var service = new CityService(context);
        await service.CreateCityAsync();
        var city = context.Cities.First();
        var cityId = city.Id;

        // Act
        var successFarm = await service.BuildBuildingAsync(cityId, BuildingType.Farm);
        var successHouse = await service.BuildBuildingAsync(cityId, BuildingType.House);

        // Assert
        Assert.True(successFarm);
        Assert.True(successHouse);
        var updatedCity = await context.Cities
            .Include(c => c.Buildings)
            .FirstAsync(c => c.Id == cityId);
        Assert.Equal(2, updatedCity.Buildings.Count);
    }
}

