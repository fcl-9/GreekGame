namespace GreekGame.Tests;

using GreekGame.API.API.Controllers;
using GreekGame.API.Application;
using GreekGame.API.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class CityControllerTests
{
    private readonly Mock<ICityService> _mockCityService;
    private readonly CitiesController _controller;

    public CityControllerTests()
    {
        _mockCityService = new Mock<ICityService>();
        _controller = new CitiesController(_mockCityService.Object);
    }

    #region Create City Tests

    [Fact]
    public async Task Create_WithValidRequest_ReturnsOkResultWithCity()
    {
        // Arrange
        var newCity = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 200m,
            LastUpdated = DateTime.UtcNow
        };

        _mockCityService
            .Setup(s => s.CreateCityAsync())
            .ReturnsAsync(newCity);

        // Act
        var result = await _controller.Create();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var returnedCity = Assert.IsType<City>(okResult.Value);
        Assert.Equal(newCity.Id, returnedCity.Id);
        Assert.Equal(newCity.Population, returnedCity.Population);
        Assert.Equal(newCity.Food, returnedCity.Food);
        Assert.Equal(newCity.Money, returnedCity.Money);
    }

    [Fact]
    public async Task Create_CallsServiceOnce()
    {
        // Arrange
        var newCity = new City
        {
            Id = Guid.NewGuid(),
            Population = 10,
            Food = 50,
            Money = 200m
        };

        _mockCityService
            .Setup(s => s.CreateCityAsync())
            .ReturnsAsync(newCity);

        // Act
        await _controller.Create();

        // Assert
        _mockCityService.Verify(s => s.CreateCityAsync(), Times.Once);
    }

    #endregion

    #region Build Building Tests

    [Fact]
    public async Task CreateBuilding_WithValidCityAndBuildingType_ReturnsOk()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var buildingType = BuildingType.Farm;

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(cityId, buildingType))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateBuilding(cityId, buildingType);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task CreateBuilding_WithValidCityAndBuildingType_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var buildingType = BuildingType.House;

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(cityId, buildingType))
            .ReturnsAsync(true);

        // Act
        await _controller.CreateBuilding(cityId, buildingType);

        // Assert
        _mockCityService.Verify(
            s => s.BuildBuildingAsync(cityId, buildingType),
            Times.Once);
    }

    [Fact]
    public async Task CreateBuilding_WhenServiceReturnsFalse_ReturnsBadRequest()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var buildingType = BuildingType.Farm;

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(cityId, buildingType))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateBuilding(cityId, buildingType);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task CreateBuilding_WithDifferentBuildingTypes_ReturnSuccessfully()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var buildingTypes = new[] { BuildingType.Farm, BuildingType.House };

        foreach (var buildingType in buildingTypes)
        {
            _mockCityService
                .Setup(s => s.BuildBuildingAsync(cityId, buildingType))
                .ReturnsAsync(true);
        }

        // Act & Assert
        foreach (var buildingType in buildingTypes)
        {
            var result = await _controller.CreateBuilding(cityId, buildingType);
            Assert.IsType<OkResult>(result);
        }
    }

    [Fact]
    public async Task CreateBuilding_WithNonExistentCity_ReturnsBadRequest()
    {
        // Arrange
        var nonExistentCityId = Guid.NewGuid();
        var buildingType = BuildingType.Farm;

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(nonExistentCityId, buildingType))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateBuilding(nonExistentCityId, buildingType);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion

    #region Upgrade Building Tests

    [Fact]
    public async Task UpgradeBuilding_WithValidBuildingId_ReturnsOk()
    {
        // Arrange
        var buildingId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.UpgradeBuildingAsync(buildingId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateBuilding(buildingId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpgradeBuilding_WithValidBuildingId_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var buildingId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.UpgradeBuildingAsync(buildingId))
            .ReturnsAsync(true);

        // Act
        await _controller.CreateBuilding(buildingId);

        // Assert
        _mockCityService.Verify(
            s => s.UpgradeBuildingAsync(buildingId),
            Times.Once);
    }

    [Fact]
    public async Task UpgradeBuilding_WhenServiceReturnsFalse_ReturnsBadRequest()
    {
        // Arrange
        var buildingId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.UpgradeBuildingAsync(buildingId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateBuilding(buildingId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpgradeBuilding_WithNonExistentBuildingId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistentBuildingId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.UpgradeBuildingAsync(nonExistentBuildingId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateBuilding(nonExistentBuildingId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    #endregion

    #region Get City Tests

    [Fact]
    public async Task GetCity_WithExistingCityId_ReturnsOkWithCity()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new City
        {
            Id = cityId,
            Population = 15,
            Food = 60,
            Money = 250m,
            LastUpdated = DateTime.UtcNow
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _controller.GetCity(cityId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCity = Assert.IsType<City>(okResult.Value);
        Assert.Equal(city.Id, returnedCity.Id);
        Assert.Equal(city.Population, returnedCity.Population);
    }

    [Fact]
    public async Task GetCity_WithNonExistentCityId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCityId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.GetCityAsync(nonExistentCityId))
            .ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetCity(nonExistentCityId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetCity_WithValidCityId_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new City { Id = cityId };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        await _controller.GetCity(cityId);

        // Assert
        _mockCityService.Verify(
            s => s.GetCityAsync(cityId),
            Times.Once);
    }

    [Fact]
    public async Task GetCity_ReturnsCompleteCity()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var buildings = new List<Building>
        {
            new() { Id = Guid.NewGuid(), CityId = cityId, Type = BuildingType.Farm, Level = 2 },
            new() { Id = Guid.NewGuid(), CityId = cityId, Type = BuildingType.House, Level = 1 }
        };
        var city = new City
        {
            Id = cityId,
            Population = 20,
            Food = 100,
            Money = 500m,
            LastUpdated = DateTime.UtcNow,
            Buildings = buildings
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _controller.GetCity(cityId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCity = Assert.IsType<City>(okResult.Value);
        Assert.Equal(2, returnedCity.Buildings.Count);
    }

    #endregion

    #region Get Events Tests

    [Fact]
    public async Task GetEvents_WithExistingCityAndActiveEvents_ReturnsOkWithEvents()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var events = new List<Event>
        {
            new() { Id = Guid.NewGuid(), CityId = cityId },
            new() { Id = Guid.NewGuid(), CityId = cityId }
        };
        var city = new City
        {
            Id = cityId,
            Population = 10,
            Food = 50,
            Money = 200m,
            ActiveEvents = events
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _controller.GetEvents(cityId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvents = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Equal(2, returnedEvents.Count);
    }

    [Fact]
    public async Task GetEvents_WithNonExistentCity_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCityId = Guid.NewGuid();

        _mockCityService
            .Setup(s => s.GetCityAsync(nonExistentCityId))
            .ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetEvents(nonExistentCityId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetEvents_WithCityButNoActiveEvents_ReturnsNotFound()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new City
        {
            Id = cityId,
            Population = 10,
            Food = 50,
            Money = 200m,
            ActiveEvents = new List<Event>()
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _controller.GetEvents(cityId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetEvents_WithCityButOneActiveEvent_ReturnsOkWithEvent()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var singleEvent = new Event { Id = Guid.NewGuid(), CityId = cityId };
        var city = new City
        {
            Id = cityId,
            Population = 10,
            Food = 50,
            Money = 200m,
            ActiveEvents = new List<Event> { singleEvent }
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _controller.GetEvents(cityId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedEvents = Assert.IsType<List<Event>>(okResult.Value);
        Assert.Single(returnedEvents);
    }

    [Fact]
    public async Task GetEvents_WithValidCityId_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new City
        {
            Id = cityId,
            ActiveEvents = new List<Event> { new() { Id = Guid.NewGuid(), CityId = cityId } }
        };

        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .ReturnsAsync(city);

        // Act
        await _controller.GetEvents(cityId);

        // Assert
        _mockCityService.Verify(
            s => s.GetCityAsync(cityId),
            Times.Once);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public async Task CreateBuilding_WithEmptyGuid_ReturnsBadRequest()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var buildingType = BuildingType.Farm;

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(emptyGuid, buildingType))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateBuilding(emptyGuid, buildingType);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetCity_WithEmptyGuid_ReturnsNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        _mockCityService
            .Setup(s => s.GetCityAsync(emptyGuid))
            .ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetCity(emptyGuid);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetEvents_WithEmptyGuid_ReturnsNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        _mockCityService
            .Setup(s => s.GetCityAsync(emptyGuid))
            .ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetEvents(emptyGuid);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task CreateCity_ThenBuildBuilding_WorksCorrectly()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var newCity = new City { Id = cityId, Population = 10, Food = 50, Money = 200m };

        _mockCityService
            .Setup(s => s.CreateCityAsync())
            .ReturnsAsync(newCity);

        _mockCityService
            .Setup(s => s.BuildBuildingAsync(cityId, BuildingType.Farm))
            .ReturnsAsync(true);

        // Act
        var createResult = await _controller.Create();
        var buildResult = await _controller.CreateBuilding(cityId, BuildingType.Farm);

        // Assert
        Assert.IsType<OkObjectResult>(createResult);
        Assert.IsType<OkResult>(buildResult);
        _mockCityService.Verify(s => s.CreateCityAsync(), Times.Once);
        _mockCityService.Verify(s => s.BuildBuildingAsync(cityId, BuildingType.Farm), Times.Once);
    }

    [Fact]
    public async Task GetCity_BeforeAndAfterBuilding_ReturnsUpdatedData()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city1 = new City { Id = cityId, Population = 10, Food = 50, Money = 200m };
        var city2 = new City
        {
            Id = cityId,
            Population = 10,
            Food = 50,
            Money = 200m,
            Buildings = new List<Building>
            {
                new() { Id = Guid.NewGuid(), Type = BuildingType.Farm, Level = 1 }
            }
        };

        var callCount = 0;
        _mockCityService
            .Setup(s => s.GetCityAsync(cityId))
            .Returns(() =>
            {
                callCount++;
                return Task.FromResult((City?)(callCount == 1 ? city1 : city2));
            });

        // Act
        var result1 = await _controller.GetCity(cityId);
        var result2 = await _controller.GetCity(cityId);

        // Assert
        var okResult1 = Assert.IsType<OkObjectResult>(result1);
        var returnedCity1 = Assert.IsType<City>(okResult1.Value);
        Assert.Empty(returnedCity1.Buildings);

        var okResult2 = Assert.IsType<OkObjectResult>(result2);
        var returnedCity2 = Assert.IsType<City>(okResult2.Value);
        Assert.Single(returnedCity2.Buildings);
    }

    #endregion
}

