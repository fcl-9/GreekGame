using GreekGame.API.Domain;

namespace GreekGame.API.Application;

public static class BuildingUpgrades
{
    public static decimal GetBuildingCost(BuildingType type) =>
        type switch
        {
            BuildingType.Farm => 50m,
            BuildingType.House => 100m,
            _ => 0m
        };

    public static decimal GetUpgradeCost(BuildingType type, int buildingLevel) =>
        type switch
        {
            BuildingType.Farm => 50m * buildingLevel,
            BuildingType.House => 100m * buildingLevel,
            _ => 0m
        };
}
