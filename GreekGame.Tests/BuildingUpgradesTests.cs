using GreekGame.API.Application;
using GreekGame.API.Domain;

namespace GreekGame.Tests;

public class BuildingUpgradesTests
{
    [Fact]
    public void GetBuildingCost_WithFarm_Returns50()
    {
        // Act
        var cost = BuildingUpgrades.GetBuildingCost(BuildingType.Farm);

        // Assert
        Assert.Equal(50m, cost);
    }

    [Fact]
    public void GetBuildingCost_WithHouse_Returns100()
    {
        // Act
        var cost = BuildingUpgrades.GetBuildingCost(BuildingType.House);

        // Assert
        Assert.Equal(100m, cost);
    }

    [Theory]
    [InlineData(BuildingType.Farm, 1, 50)]
    [InlineData(BuildingType.Farm, 2, 100)]
    [InlineData(BuildingType.Farm, 5, 250)]
    [InlineData(BuildingType.House, 1, 100)]
    [InlineData(BuildingType.House, 2, 200)]
    [InlineData(BuildingType.House, 3, 300)]
    public void GetUpgradeCost_WithValidBuildingTypeAndLevel_ReturnsCorrectCost(
        BuildingType type, int level, decimal expectedCost)
    {
        // Act
        var cost = BuildingUpgrades.GetUpgradeCost(type, level);

        // Assert
        Assert.Equal(expectedCost, cost);
    }

    [Fact]
    public void GetUpgradeCost_WithLevel0_ReturnsZero()
    {
        // Act
        var farmCost = BuildingUpgrades.GetUpgradeCost(BuildingType.Farm, 0);
        var houseCost = BuildingUpgrades.GetUpgradeCost(BuildingType.House, 0);

        // Assert
        Assert.Equal(0m, farmCost);
        Assert.Equal(0m, houseCost);
    }
}
